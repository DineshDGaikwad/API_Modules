using APIPropertyRegistry.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IPropertyOwnershipService
    {
        Task<IEnumerable<PropertyOwnershipResponseDto>> GetAllAsync();
        Task<PropertyOwnershipResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PropertyOwnershipResponseDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<PropertyOwnershipResponseDto>> GetByPropertyIdAsync(int propertyId);
        Task<bool> CreateAsync(PropertyOwnershipCreateDto dto);
        Task<bool> VerifyOwnershipAsync(PropertyOwnershipVerifyDto dto);
        Task<bool> TransferOwnershipAsync(PropertyOwnershipTransferDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
