using APIPropertyRegistry.Data;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByIdNoTrackingAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
{
    if (string.IsNullOrWhiteSpace(role))
        return new List<User>();

    var normalizedRole = role.Trim().ToLower();

    return await _context.Users
        .Where(u => (u.Role ?? string.Empty).ToLower() == normalizedRole)
        .AsNoTracking()
        .ToListAsync();
}


        public async Task<IEnumerable<User>> GetPendingAgentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role.ToLower() == "agent" && !u.IsApproved)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchAsync(string query, string? role)
        {
            var trimmedQuery = query?.Trim();
            var normalizedRole = role?.Trim().ToLower();

            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(normalizedRole))
                users = users.Where(u => u.Role.ToLower() == normalizedRole);

            if (!string.IsNullOrWhiteSpace(trimmedQuery))
            {
                var pattern = $"%{trimmedQuery}%";
                users = users.Where(u => EF.Functions.Like(u.FullName, pattern) || EF.Functions.Like(u.Email, pattern));
            }

            return await users
                .OrderByDescending(u => u.CreatedAt)
                .Take(100)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
                _context.Users.Remove(user);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
