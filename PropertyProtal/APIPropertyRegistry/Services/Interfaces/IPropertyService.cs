using APIPropertyRegistry.DTOs.PropertyDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyResponseDto>> GetAllAsync();
        Task<PropertyResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PropertyResponseDto>> GetByOwnerIdAsync(int ownerId);
        Task<IEnumerable<PropertyResponseDto>> GetPendingAsync();
        Task<IEnumerable<PropertyResponseDto>> GetApprovedAsync();
        Task<IEnumerable<PropertyResponseDto>> GetForSaleAsync();
        Task<IEnumerable<PropertyResponseDto>> GetAvailablePropertiesAsync();
        Task<IEnumerable<PropertyResponseDto>> SearchAsync(string? query, string? status, string? city, decimal? minPrice, decimal? maxPrice);

        Task<PropertyResponseDto?> CreateAsync(PropertyCreateDto dto);
        Task<bool> UpdateAsync(int id, PropertyUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ApprovePropertyAsync(int propertyId, bool approve, int adminId, string? remarks);
        Task<bool> MarkPropertyForSaleAsync(PropertySellDto dto);
Task<bool> RemovePropertyFromSaleAsync(int propertyId, int ownerId);
    }
}
