using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Implementations
{
    public class PropertyTransactionService : IPropertyTransactionService
    {
        private const string Pending = "Pending";
        private const string PendingAdmin = "Pending Admin";
        private const string Approved = "Approved";
        private const string RejectedByAgent = "Rejected by Agent";
        private const string RejectedByAdmin = "Rejected by Admin";
        private const string Cancelled = "Cancelled";

        private const string AgentStage = "AgentReview";
        private const string AdminStage = "AdminReview";
        private const string CompletedStage = "Completed";
        private const string ClosedStage = "Closed";

        private readonly IPropertyTransactionRepository _repo;
        private readonly IPropertyOwnershipService _ownershipService;
        private readonly IAgentPropertyRepository _agentPropertyRepo;
        private readonly IPropertyRepository _propertyRepo;

        public PropertyTransactionService(
            IPropertyTransactionRepository repo,
            IPropertyOwnershipService ownershipService,
            IAgentPropertyRepository agentPropertyRepo,
            IPropertyRepository propertyRepo)
        {
            _repo = repo;
            _ownershipService = ownershipService;
            _agentPropertyRepo = agentPropertyRepo;
            _propertyRepo = propertyRepo;
        }

        public async Task<PropertyTransactionResponseDto> CreateTransactionAsync(PropertyTransactionCreateDto dto)
        {
            var property = await _propertyRepo.GetByIdAsync(dto.PropertyId)
                ?? throw new InvalidOperationException("Property not found.");

            if (!property.IsApproved)
                throw new InvalidOperationException("Property must be approved before purchase.");

            if (!property.IsAvailable)
                throw new InvalidOperationException("Property is no longer available.");

            if (property.OwnerId.HasValue && property.OwnerId.Value == dto.BuyerId)
                throw new InvalidOperationException("You cannot buy a property you own.");

            if (property.OwnerId.HasValue && property.OwnerId.Value != dto.SellerId)
                throw new InvalidOperationException("Only the owner can initiate a sale for this property.");

            if (await _repo.HasActiveTransactionAsync(dto.PropertyId))
                throw new InvalidOperationException("An active buy request already exists for this property.");

            var activeAgentId = await _agentPropertyRepo.GetActiveAgentIdByPropertyAsync(dto.PropertyId);

            int? agentId = dto.AgentId;

            if (!agentId.HasValue)
            {
                agentId = activeAgentId ?? property.AgentId;
                if (!agentId.HasValue)
                    throw new InvalidOperationException("Assign an agent before selling this property.");
            }
            else
            {
                if (activeAgentId.HasValue && activeAgentId.Value != agentId.Value)
                    throw new InvalidOperationException("This property is assigned to a different agent.");

                if (property.AgentId.HasValue && property.AgentId.Value != agentId.Value)
                    throw new InvalidOperationException("This property is assigned to a different agent.");
            }

            var now = DateTime.UtcNow;

            var transaction = new PropertyTransaction
            {
                PropertyId = dto.PropertyId,
                SellerId = dto.SellerId,
                BuyerId = dto.BuyerId,
                AgentId = agentId.Value,
                Amount = dto.TransactionAmount,
                AgentCommission = dto.AgentCommission,
                TransactionDate = dto.TransactionDate ?? now,
                Status = Pending,
                Stage = AgentStage,
                CreatedAt = now,
                UpdatedAt = now
            };

            var created = await _repo.CreateAsync(transaction);
            var populated = await _repo.GetByIdAsync(created.TransactionId) ?? created;
            return MapToResponse(populated);
        }

        public async Task<IEnumerable<PropertyTransactionResponseDto>> GetAllAsync()
        {
            var transactions = await _repo.GetAllAsync();
            return transactions.Select(MapToResponse);
        }

        public async Task<PropertyTransactionResponseDto?> GetByIdAsync(int id)
        {
            var transaction = await _repo.GetByIdAsync(id);
            return transaction == null ? null : MapToResponse(transaction);
        }

        public async Task<IEnumerable<PropertyTransactionResponseDto>> GetByBuyerAsync(int buyerId)
        {
            var transactions = await _repo.GetByBuyerAsync(buyerId);
            return transactions.Select(MapToResponse);
        }

        public async Task<IEnumerable<PropertyTransactionResponseDto>> GetBySellerAsync(int sellerId)
        {
            var transactions = await _repo.GetBySellerAsync(sellerId);
            return transactions.Select(MapToResponse);
        }

        public async Task<IEnumerable<PropertyTransactionResponseDto>> GetByAgentAsync(int agentId, string? status = null, bool includeArchived = false)
        {
            var transactions = await _repo.GetByAgentAsync(agentId, status, includeArchived);
            return transactions.Select(MapToResponse);
        }

        public async Task<IEnumerable<PropertyTransactionResponseDto>> GetPendingForAdminAsync()
        {
            var transactions = await _repo.GetPendingForAdminAsync();
            return transactions.Select(MapToResponse);
        }

        public async Task<bool> SubmitAgentDecisionAsync(AgentTransactionDecisionDto dto)
        {
            var transaction = await _repo.GetByIdAsync(dto.TransactionId);
            if (transaction == null || transaction.AgentId != dto.AgentId)
                return false;

            if (!string.Equals(transaction.Status, Pending, StringComparison.OrdinalIgnoreCase) || !string.Equals(transaction.Stage, AgentStage, StringComparison.OrdinalIgnoreCase))
                return false;

            var now = DateTime.UtcNow;
            transaction.AgentDecisionDate = now;
            transaction.AgentRemarks = dto.Remarks;
            transaction.UpdatedAt = now;

            if (dto.Approve)
            {
                transaction.Status = PendingAdmin;
                transaction.Stage = AdminStage;
                transaction.IsArchived = false;
                transaction.ArchivedDate = null;
            }
            else
            {
                transaction.Status = RejectedByAgent;
                transaction.Stage = ClosedStage;
                transaction.IsArchived = true;
                transaction.ArchivedDate = now;
            }

            await _repo.UpdateAsync(transaction);
            return true;
        }

        public async Task<bool> SubmitAdminDecisionAsync(AdminTransactionDecisionDto dto)
        {
            var transaction = await _repo.GetByIdAsync(dto.TransactionId);
            if (transaction == null)
                return false;

            if (!string.Equals(transaction.Status, PendingAdmin, StringComparison.OrdinalIgnoreCase) || !string.Equals(transaction.Stage, AdminStage, StringComparison.OrdinalIgnoreCase))
                return false;

            if (dto.AdminId <= 0)
                throw new InvalidOperationException("Admin identifier is required to approve transactions.");

            var adminId = dto.AdminId;
            var now = DateTime.UtcNow;
            transaction.AdminDecisionDate = now;
            transaction.AdminRemarks = dto.Remarks;
            transaction.VerifiedBy = adminId;
            transaction.VerifiedDate = now;
            transaction.UpdatedAt = now;
            transaction.Property = null;
            transaction.Seller = null;
            transaction.Buyer = null;
            transaction.Agent = null;
            transaction.Verifier = null;

            if (dto.Approve)
            {
                transaction.Status = Approved;
                transaction.Stage = CompletedStage;
                transaction.IsArchived = false;
                transaction.ArchivedDate = null;

                var property = await _propertyRepo.GetByIdForUpdateAsync(transaction.PropertyId)
                    ?? throw new InvalidOperationException("Property not found for transaction.");

                property.OwnerId = transaction.BuyerId;
                property.IsAvailable = false;
                property.IsForSale = false;
                property.Status = "Sold";
                property.UpdatedAt = now;

                await _propertyRepo.UpdateAsync(property);

                var transferSucceeded = await _ownershipService.TransferOwnershipAsync(new PropertyOwnershipTransferDto
                {
                    PropertyId = transaction.PropertyId,
                    NewOwnerId = transaction.BuyerId,
                    VerifierId = adminId,
                    TransferDate = now
                });

                if (!transferSucceeded)
                    throw new InvalidOperationException("Failed to transfer property ownership.");

                var competing = await _repo.GetPendingByPropertyAsync(transaction.PropertyId);
                var toCancel = competing.Where(t => t.TransactionId != transaction.TransactionId).ToList();
                if (toCancel.Count > 0)
                {
                    foreach (var other in toCancel)
                    {
                        other.Status = Cancelled;
                        other.Stage = ClosedStage;
                        other.IsArchived = true;
                        other.ArchivedDate = now;
                        other.UpdatedAt = now;
                        other.VerifiedBy = adminId;
                        other.VerifiedDate = now;
                    }

                    await _repo.UpdateRangeAsync(toCancel);
                }
            }
            else
            {
                transaction.Status = RejectedByAdmin;
                transaction.Stage = ClosedStage;
                transaction.IsArchived = true;
                transaction.ArchivedDate = now;
            }

            await _repo.UpdateAsync(transaction);
            return true;
        }

        public Task<bool> VerifyTransactionAsync(PropertyTransactionVerifyDto dto)
        {
            var adminDto = new AdminTransactionDecisionDto
            {
                TransactionId = dto.TransactionId,
                AdminId = dto.VerifierId,
                Approve = dto.Approve
            };

            return SubmitAdminDecisionAsync(adminDto);
        }

        private static PropertyTransactionResponseDto MapToResponse(PropertyTransaction transaction)
        {
            return new PropertyTransactionResponseDto
            {
                TransactionId = transaction.TransactionId,
                PropertyId = transaction.PropertyId,
                PropertyNumber = transaction.Property?.PropertyNumber ?? string.Empty,
                PropertyTitle = transaction.Property?.Title ?? "Unknown",
                PropertyAddress = transaction.Property?.Address ?? string.Empty,
                PropertyCity = transaction.Property?.City ?? string.Empty,
                PropertyPrice = transaction.Property?.Price ?? 0,
                PropertyImageUrl = transaction.Property?.Documents?
                    .OrderByDescending(d => d.UploadDate)
                    .FirstOrDefault()?.FilePath,
                SellerId = transaction.SellerId,
                SellerName = transaction.Seller?.FullName ?? "Unknown",
                BuyerId = transaction.BuyerId,
                BuyerName = transaction.Buyer?.FullName ?? "Unknown",
                AgentId = transaction.AgentId,
                AgentName = transaction.Agent?.FullName ?? "N/A",
                TransactionAmount = transaction.Amount,
                AgentCommission = transaction.AgentCommission,
                Status = transaction.Status,
                Stage = transaction.Stage,
                IsArchived = transaction.IsArchived,
                TransactionDate = transaction.TransactionDate,
                AgentDecisionDate = transaction.AgentDecisionDate,
                AdminDecisionDate = transaction.AdminDecisionDate,
                ArchivedDate = transaction.ArchivedDate,
                VerifiedDate = transaction.VerifiedDate,
                AgentRemarks = transaction.AgentRemarks,
                AdminRemarks = transaction.AdminRemarks
            };
        }
    }
}
