using APIPropertyRegistry.Data;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Implementations
{
    public class PropertyOwnershipRepository : IPropertyOwnershipRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyOwnershipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PropertyOwnership?> GetByIdAsync(int id)
        {
            return await _context.PropertyOwnerships
                .Include(o => o.Property)
                .Include(o => o.User)
                .Include(o => o.Verifier)
                .FirstOrDefaultAsync(o => o.OwnershipId == id);
        }

        public async Task<IEnumerable<PropertyOwnership>> GetAllAsync()
        {
            return await _context.PropertyOwnerships
                .Include(o => o.Property)
                .Include(o => o.User)
                .Include(o => o.Verifier)
                .OrderByDescending(o => o.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyOwnership>> GetByUserIdAsync(int userId)
        {
            return await _context.PropertyOwnerships
                .Include(o => o.Property)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyOwnership>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.PropertyOwnerships
                .Include(o => o.User)
                .Where(o => o.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task AddAsync(PropertyOwnership ownership)
        {
            await _context.PropertyOwnerships.AddAsync(ownership);
        }

        public Task UpdateAsync(PropertyOwnership ownership)
        {
            _context.PropertyOwnerships.Update(ownership);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _context.PropertyOwnerships.FindAsync(id);
            if (record != null)
                _context.PropertyOwnerships.Remove(record);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
