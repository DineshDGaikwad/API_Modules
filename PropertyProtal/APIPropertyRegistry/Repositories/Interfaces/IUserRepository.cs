using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByIdNoTrackingAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> GetPendingAgentsAsync();
        Task<IEnumerable<User>> SearchAsync(string query, string? role);

        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
