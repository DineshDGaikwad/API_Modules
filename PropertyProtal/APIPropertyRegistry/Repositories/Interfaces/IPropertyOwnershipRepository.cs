using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Interfaces
{
    public interface IPropertyOwnershipRepository
    {
        Task<PropertyOwnership?> GetByIdAsync(int id);
        Task<IEnumerable<PropertyOwnership>> GetAllAsync();
        Task<IEnumerable<PropertyOwnership>> GetByUserIdAsync(int userId);
        Task<IEnumerable<PropertyOwnership>> GetByPropertyIdAsync(int propertyId);
        Task AddAsync(PropertyOwnership ownership);
        Task UpdateAsync(PropertyOwnership ownership);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
