using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Interfaces
{
    public interface IPropertyRepository
    {
        Task<Property?> GetByIdAsync(int id);
        Task<Property?> GetByIdForUpdateAsync(int id);
        Task<IEnumerable<Property>> GetAllAsync();
        Task<IEnumerable<Property>> GetByOwnerIdAsync(int ownerId);
        Task<IEnumerable<Property>> GetPendingAsync();
        Task<IEnumerable<Property>> GetApprovedAsync();
        Task<IEnumerable<Property>> GetForSaleAsync();
        Task<IEnumerable<Property>> GetAvailableAsync();
        Task<IEnumerable<Property>> SearchAsync(string? query, string? status, string? city, decimal? minPrice, decimal? maxPrice);
        Task<bool> PropertyNumberExistsAsync(string propertyNumber);

        Task AddAsync(Property property);
        Task UpdateAsync(Property property);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
