using APIPropertyRegistry.Data;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetPendingAgentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "agent" && u.IsApproved == false)
                .OrderByDescending(u => u.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetApprovedAgentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "agent" && u.IsApproved == true)
                .OrderByDescending(u => u.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetAgentByIdAsync(int id)
        {
            return await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.UserId == id && u.Role == "agent");
        }

        public async Task UpdateAgentAsync(User agent)
        {
            _context.Users.Update(agent);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Property>> GetPendingPropertiesAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Where(p => p.IsApproved == false)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetApprovedPropertiesAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Where(p => p.IsApproved == true)
                .OrderByDescending(p => p.ApprovedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .AsTracking()
                .FirstOrDefaultAsync(p => p.PropertyId == id);
        }

        public async Task UpdatePropertyAsync(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(string? role = null)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role.ToLower() == role.ToLower());
            return await query
                .OrderByDescending(u => u.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAgentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role.ToLower() == "agent")
                .OrderByDescending(u => u.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            var property = await GetPropertyByIdAsync(id);
            if (property == null) return false;
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PropertyTransaction>> GetAllTransactionsAsync(int limit = 100)
        {
            return await _context.PropertyTransactions
                .Include(t => t.Property)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Agent)
                .OrderByDescending(t => t.TransactionDate)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyTransaction>> GetRecentTransactionsAsync(int limit = 20)
        {
            return await _context.PropertyTransactions
                .Include(t => t.Property)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Agent)
                .OrderByDescending(t => t.TransactionDate)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string query)
        {
            var q = query.ToLower();
            return await _context.Users
                .Where(u => u.Role.ToLower() == "user" &&
                    (u.FullName.ToLower().Contains(q) ||
                     u.Email.ToLower().Contains(q)))
                .OrderBy(u => u.FullName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchAgentsAsync(string query)
        {
            var q = query.ToLower();
            return await _context.Users
                .Where(u => u.Role.ToLower() == "agent" &&
                    (u.FullName.ToLower().Contains(q) ||
                     u.Email.ToLower().Contains(q)))
                .OrderBy(u => u.FullName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string query)
        {
            var q = query.ToLower();
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Where(p => p.Title.ToLower().Contains(q) ||
                           p.Address.ToLower().Contains(q) ||
                           p.PropertyNumber.ToLower().Contains(q) ||
                           p.City.ToLower().Contains(q))
                .OrderBy(p => p.Title)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
