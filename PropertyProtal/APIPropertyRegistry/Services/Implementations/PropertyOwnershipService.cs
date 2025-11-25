using APIPropertyRegistry.Data;
using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Implementations
{
    public class PropertyOwnershipService : IPropertyOwnershipService
    {
        private readonly IPropertyOwnershipRepository _repository;
        private readonly ApplicationDbContext _context;
        public PropertyOwnershipService(IPropertyOwnershipRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<PropertyOwnershipResponseDto>> GetAllAsync()
        {
            var records = await _repository.GetAllAsync();
            return records.Select(MapToResponse);
        }

        public async Task<PropertyOwnershipResponseDto?> GetByIdAsync(int id)
        {
            var record = await _repository.GetByIdAsync(id);
            return record == null ? null : MapToResponse(record);
        }

        public async Task<IEnumerable<PropertyOwnershipResponseDto>> GetByUserIdAsync(int userId)
        {
            var records = await _repository.GetByUserIdAsync(userId);
            return records.Select(MapToResponse);
        }

        public async Task<IEnumerable<PropertyOwnershipResponseDto>> GetByPropertyIdAsync(int propertyId)
        {
            var records = await _repository.GetByPropertyIdAsync(propertyId);
            return records.Select(MapToResponse);
        }

        public async Task<bool> CreateAsync(PropertyOwnershipCreateDto dto)
        {
            var property = await _context.Set<Property>().FindAsync(dto.PropertyId);
            if (property == null)
                throw new InvalidOperationException("Property not found.");

            var ownership = new PropertyOwnership
            {
                PropertyId = dto.PropertyId,
                UserId = dto.OwnerId,
                StartDate = dto.OwnershipStartDate,
                Verified = false,
                Status = "Pending"
            };

            await _repository.AddAsync(ownership);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> VerifyOwnershipAsync(PropertyOwnershipVerifyDto dto)
        {
            var record = await _repository.GetByIdAsync(dto.OwnershipId);
            if (record == null) return false;

            record.Verified = dto.Verified;
            record.VerifiedBy = dto.VerifierId;
            record.VerifiedDate = DateTime.UtcNow;
            record.Status = dto.Verified ? "Approved" : "Rejected";

            await _repository.UpdateAsync(record);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> TransferOwnershipAsync(PropertyOwnershipTransferDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = await _repository.GetByPropertyIdAsync(dto.PropertyId);

                foreach (var record in existing.Where(r => !r.EndDate.HasValue))
                {
                    record.EndDate = dto.TransferDate;
                    record.Status = "Transferred";
                    await _repository.UpdateAsync(record);
                }

                var newOwnership = new PropertyOwnership
                {
                    PropertyId = dto.PropertyId,
                    UserId = dto.NewOwnerId,
                    StartDate = dto.TransferDate,
                    Verified = true,
                    VerifiedBy = dto.VerifierId,
                    VerifiedDate = dto.TransferDate,
                    Status = "Approved"
                };

                await _repository.AddAsync(newOwnership);
                await _repository.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return await _repository.SaveChangesAsync();
        }

        private static PropertyOwnershipResponseDto MapToResponse(PropertyOwnership o)
        {
            return new PropertyOwnershipResponseDto
            {
                OwnershipId = o.OwnershipId,
                PropertyId = o.PropertyId,
                PropertyTitle = o.Property?.Title ?? "Unknown",
                OwnerId = o.UserId,
                OwnerName = o.User?.FullName ?? "Unknown",
                OwnershipType = o.OwnershipType,
                Verified = o.Verified,
                Status = o.Status,
                OwnershipStartDate = o.StartDate,
                VerifiedDate = o.VerifiedDate
            };
        }
    }
}
