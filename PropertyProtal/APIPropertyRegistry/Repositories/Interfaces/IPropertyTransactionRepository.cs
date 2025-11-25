using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Interfaces
{
    public interface IPropertyTransactionRepository
    {
        Task<PropertyTransaction> CreateAsync(PropertyTransaction transaction);
        Task<IEnumerable<PropertyTransaction>> GetAllAsync();
        Task<PropertyTransaction?> GetByIdAsync(int id);
        Task<IEnumerable<PropertyTransaction>> GetByBuyerAsync(int buyerId);
        Task<IEnumerable<PropertyTransaction>> GetBySellerAsync(int sellerId);
        Task<IEnumerable<PropertyTransaction>> GetByAgentAsync(int agentId, string? status = null, bool includeArchived = false);
        Task<IEnumerable<PropertyTransaction>> GetPendingForAdminAsync();
        Task<bool> HasActiveTransactionAsync(int propertyId);
        Task<IEnumerable<PropertyTransaction>> GetPendingByPropertyAsync(int propertyId);
        Task UpdateAsync(PropertyTransaction transaction);
        Task UpdateRangeAsync(IEnumerable<PropertyTransaction> transactions);
        Task<bool> SaveChangesAsync();
    }
}
