using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<IEnumerable<User>> GetPendingAgentsAsync();
        Task<IEnumerable<User>> GetApprovedAgentsAsync();
        Task<User?> GetAgentByIdAsync(int id);
        Task UpdateAgentAsync(User agent);

        Task<IEnumerable<Property>> GetPendingPropertiesAsync();
        Task<IEnumerable<Property>> GetApprovedPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(int id);
        Task UpdatePropertyAsync(Property property);

        Task<IEnumerable<User>> GetAllUsersAsync(string? role = null);
        Task<IEnumerable<User>> GetAllAgentsAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);

        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<bool> DeletePropertyAsync(int id);

        Task<IEnumerable<PropertyTransaction>> GetAllTransactionsAsync(int limit = 100);
        Task<IEnumerable<PropertyTransaction>> GetRecentTransactionsAsync(int limit = 20);

        Task<IEnumerable<User>> SearchUsersAsync(string query);
        Task<IEnumerable<User>> SearchAgentsAsync(string query);
        Task<IEnumerable<Property>> SearchPropertiesAsync(string query);
    }
}
