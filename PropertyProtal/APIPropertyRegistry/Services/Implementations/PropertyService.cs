using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.DTOs.PropertyDtos;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace APIPropertyRegistry.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repository;
        private readonly IDocumentService _documentService;
        private readonly IAgentPropertyRepository _agentPropertyRepository;
        private readonly IPropertyOwnershipRepository _ownershipRepository;
        private readonly IUserRepository _userRepository;
        private readonly PropertyNumberGeneratorService _propertyNumberGenerator;

        public PropertyService(
            IPropertyRepository repository,
            IDocumentService documentService,
            IAgentPropertyRepository agentPropertyRepository,
            IPropertyOwnershipRepository ownershipRepository,
            PropertyNumberGeneratorService propertyNumberGenerator,
            IUserRepository userRepository)
        {
            _repository = repository;
            _documentService = documentService;
            _agentPropertyRepository = agentPropertyRepository;
            _ownershipRepository = ownershipRepository;
            _propertyNumberGenerator = propertyNumberGenerator;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetAllAsync()
        {
            var properties = await _repository.GetAllAsync();
            return properties.Select(p => MapToResponse(p));

        }

        public async Task<PropertyResponseDto?> GetByIdAsync(int id)
        {
            var property = await _repository.GetByIdAsync(id);
            if (property == null)
                return null;

            var documents = await _documentService.GetByPropertyAsync(property.PropertyId);
            return MapToResponse(property, documents);
        }

        public async Task<IEnumerable<PropertyResponseDto>> GetByOwnerIdAsync(int ownerId)
        {
            var properties = await _repository.GetByOwnerIdAsync(ownerId);
            return properties.Select(p => MapToResponse(p));

        }

        public async Task<IEnumerable<PropertyResponseDto>> GetPendingAsync()
        {
            var properties = await _repository.GetPendingAsync();
            return properties.Select(p => MapToResponse(p));

        }

        public async Task<IEnumerable<PropertyResponseDto>> GetApprovedAsync()
        {
            var properties = await _repository.GetApprovedAsync();
            return properties.Select(p => MapToResponse(p));

        }

        public async Task<IEnumerable<PropertyResponseDto>> GetForSaleAsync()
        {
            var properties = await _repository.GetForSaleAsync();
            return properties.Select(p => MapToResponse(p));

        }

        public async Task<IEnumerable<PropertyResponseDto>> GetAvailablePropertiesAsync()
        {
            var properties = await _repository.GetAvailableAsync();
            return properties.Select(p => MapToResponse(p));
        }

        public async Task<IEnumerable<PropertyResponseDto>> SearchAsync(string? query, string? status, string? city, decimal? minPrice, decimal? maxPrice)
        {
            var properties = await _repository.SearchAsync(query, status, city, minPrice, maxPrice);
            return properties.Select(p => MapToResponse(p));

        }

        public async Task<PropertyResponseDto?> CreateAsync(PropertyCreateDto dto)
        {
            var documentFiles = dto.Documents?
                .Where(f => f != null && f.Length > 0)
                .ToList() ?? new List<IFormFile>();

            if (!documentFiles.Any())
                throw new InvalidOperationException("At least one document is required for property submission.");

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var property = new Property
            {
                Title = dto.Title,
                Description = dto.Description,
                Address = dto.Address,
                City = dto.City,
                Area = dto.Area,
                Price = dto.Price,
                OwnerId = dto.OwnerId,
                CreatedBy = dto.CreatedBy,
                IsApproved = false,
                IsAvailable = false,
                IsForSale = false,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(property);
            await _repository.SaveChangesAsync();

            var createdDocs = new List<DocumentResponseDto>();

            foreach (var file in documentFiles)
            {
                var documentDto = new DocumentCreateDto
                {
                    PropertyId = property.PropertyId,
                    UploadedBy = dto.CreatedBy,
                    DocumentType = "Ownership Proof",
                    DocumentName = file.FileName
                };

                var savedDoc = await _documentService.CreateAsync(documentDto, file);
                createdDocs.Add(savedDoc);
            }

            transaction.Complete();
            return MapToResponse(property, createdDocs);
        }

        public async Task<bool> UpdateAsync(int id, PropertyUpdateDto dto)
        {
            var property = await _repository.GetByIdAsync(id);
            if (property == null)
                return false;

            property.Title = dto.Title ?? property.Title;
            property.Description = dto.Description ?? property.Description;
            property.Address = dto.Address ?? property.Address;
            property.City = dto.City ?? property.City;
            property.Area = dto.Area ?? property.Area;
            property.Price = dto.Price ?? property.Price;
            property.IsAvailable = dto.IsAvailable ?? property.IsAvailable;
            property.Remarks = dto.Remarks ?? property.Remarks;
            property.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(property);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> ApprovePropertyAsync(int propertyId, bool approve, int adminId, string? remarks)
        {
            var property = await _repository.GetByIdAsync(propertyId);
            if (property == null)
                return false;

            if (property.IsApproved && approve)
                return true;

            property.IsApproved = approve;
            property.ApprovedBy = adminId;
            property.ApprovedDate = DateTime.UtcNow;
            property.Status = approve ? "Approved" : "Rejected";
            property.Remarks = remarks ?? property.Remarks;

            if (approve && string.IsNullOrEmpty(property.PropertyNumber))
            {
                property.PropertyNumber = await GenerateUniquePropertyNumberAsync();
            }

            await _repository.UpdateAsync(property);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> MarkPropertyForSaleAsync(PropertySellDto dto)
        {
            if (dto.AgentId <= 0)
                throw new InvalidOperationException("Select an approved agent.");

            if (_userRepository == null)
                throw new InvalidOperationException("Unable to validate agent.");

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var property = await _repository.GetByIdForUpdateAsync(dto.PropertyId)
                ?? throw new InvalidOperationException("Property not found.");

            if (!property.IsApproved)
                throw new InvalidOperationException("Property must be approved before listing for sale.");

            var owner = await _userRepository.GetUserByIdNoTrackingAsync(dto.OwnerId)
                ?? throw new InvalidOperationException("Owner not found.");

            if (property.OwnerId != owner.UserId)
                throw new InvalidOperationException("Only the verified owner can mark this property for sale.");

            var agent = await _userRepository.GetUserByIdNoTrackingAsync(dto.AgentId)
                ?? throw new InvalidOperationException("Select an approved agent.");

            if (!agent.IsApproved || !agent.Role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Select an approved agent.");

            var remarks = string.IsNullOrWhiteSpace(dto.Remarks)
                ? "Listing property for sale."
                : dto.Remarks.Trim();

            property.AgentId = dto.AgentId;
            property.IsForSale = true;
            property.IsAvailable = true;
            property.Status = "Listed for Sale";
            property.SaleListedDate = DateTime.UtcNow;
            property.Remarks = remarks;
            property.UpdatedAt = DateTime.UtcNow;

            var assignments = (await _agentPropertyRepository.GetByPropertyAsync(dto.PropertyId, track: true)).ToList();
            var hasSelectedAssignment = false;

            foreach (var assignment in assignments)
            {
                var isSelected = assignment.AgentId == dto.AgentId;
                if (isSelected)
                    hasSelectedAssignment = true;

                assignment.Status = isSelected ? "Active" : "Revoked";
                assignment.IsApproved = isSelected;
                assignment.AssignedDate = DateTime.UtcNow;
                assignment.Notes = remarks;
            }

            if (!hasSelectedAssignment)
            {
                await _agentPropertyRepository.AddAsync(new AgentProperty
                {
                    AgentId = dto.AgentId,
                    PropertyId = property.PropertyId,
                    AssignedDate = DateTime.UtcNow,
                    Status = "Active",
                    IsApproved = true,
                    Notes = remarks
                });
            }

            await _repository.SaveChangesAsync();
            transaction.Complete();
            return true;
        }

        public async Task<bool> RemovePropertyFromSaleAsync(int propertyId, int ownerId)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var property = await _repository.GetByIdForUpdateAsync(propertyId)
                ?? throw new InvalidOperationException("Property not found.");

            if (property.OwnerId != ownerId)
                throw new InvalidOperationException("Only the verified owner can remove this property from sale.");

            if (!property.IsForSale)
                throw new InvalidOperationException("Property is not currently listed for sale.");

            property.IsForSale = false;
            property.IsAvailable = false;
            property.Status = property.IsApproved ? "Approved" : property.Status;
            property.AgentId = null;
            property.SaleListedDate = null;
            property.Remarks = "Removed from sale by owner";
            property.UpdatedAt = DateTime.UtcNow;

            var assignments = await _agentPropertyRepository.GetByPropertyAsync(propertyId);

            foreach (var assignment in assignments)
            {
                assignment.Status = "Revoked";
                assignment.IsApproved = false;
                assignment.AssignedDate = DateTime.UtcNow;
                assignment.Notes = "Listing removed by owner";

                await _agentPropertyRepository.UpdateAsync(assignment);
            }

            await _repository.SaveChangesAsync();
            transaction.Complete();
            return true;
        }


        private async Task<string> GenerateUniquePropertyNumberAsync()
        {
            string propertyNumber;
            do
            {
                propertyNumber = _propertyNumberGenerator.GeneratePropertyNumber();
            }
            while (await _repository.PropertyNumberExistsAsync(propertyNumber));

            return propertyNumber;
        }

        private static PropertyResponseDto MapToResponse(Property property, IEnumerable<DocumentResponseDto>? documents = null)
        {
            var docs = documents?.ToList()
                ?? property.Documents?.Select(d => MapDocument(d, property.Title)).ToList()
                ?? new List<DocumentResponseDto>();

            return new PropertyResponseDto
            {
                PropertyId = property.PropertyId,
                PropertyNumber = property.PropertyNumber,
                Title = property.Title,
                Description = property.Description,
                Address = property.Address,
                City = property.City,
                Area = property.Area,
                Price = property.Price,
                IsApproved = property.IsApproved,
                IsAvailable = property.IsAvailable,
                IsForSale = property.IsForSale,
                Status = property.Status,
                Remarks = property.Remarks,
                OwnerId = property.OwnerId ?? 0,
                OwnerName = property.Owner?.FullName ?? string.Empty,
                OwnerEmail = property.Owner?.Email ?? string.Empty,
                CreatedBy = property.CreatedBy,
                AgentId = property.AgentId,
                AgentName = property.Agent?.FullName ?? string.Empty,
                AgentEmail = property.Agent?.Email ?? string.Empty,
                CreatedAt = property.CreatedAt,
                SaleListedDate = property.SaleListedDate,
                Documents = docs
            };
        }

        private static DocumentResponseDto MapDocument(Document document, string propertyTitle)
        {
            return new DocumentResponseDto
            {
                DocumentId = document.DocumentId,
                PropertyId = document.PropertyId,
                PropertyTitle = propertyTitle,
                FileName = document.FileName,
                DocumentType = document.DocumentType,
                FilePath = document.FilePath,
                UploadedBy = document.UploadedBy,
                UploaderName = document.Uploader?.FullName ?? string.Empty,
                Verified = document.Verified,
                VerifiedBy = document.VerifiedBy,
                VerifierName = document.Verifier?.FullName,
                UploadDate = document.UploadDate,
                VerifiedDate = document.VerifiedDate
            };
        }
    }
}
