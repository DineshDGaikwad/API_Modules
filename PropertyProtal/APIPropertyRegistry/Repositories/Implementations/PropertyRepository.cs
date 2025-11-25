using APIPropertyRegistry.Data;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Implementations
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Property?> GetByIdAsync(int id)
{
    return await _context.Properties
        .Include(p => p.Owner)
        .Include(p => p.Agent)
        .Include(p => p.Approver)
        .Include(p => p.Documents)
            .ThenInclude(d => d.Uploader)
        .Include(p => p.Documents)
            .ThenInclude(d => d.Verifier)
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.PropertyId == id);
}

        public async Task<Property?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Properties
                .AsTracking()
                .FirstOrDefaultAsync(p => p.PropertyId == id);
        }


        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Include(p => p.Documents).ThenInclude(d => d.Uploader)
                .Include(p => p.Documents).ThenInclude(d => d.Verifier)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByOwnerIdAsync(int ownerId)
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Include(p => p.Documents).ThenInclude(d => d.Uploader)
                .Include(p => p.Documents).ThenInclude(d => d.Verifier)
                .Where(p => p.OwnerId == ownerId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPendingAsync()
        {
            return await _context.Properties
                .Include(p => p.Agent)
                .Include(p => p.Owner)
                .Include(p => p.Documents).ThenInclude(d => d.Uploader)
                .Include(p => p.Documents).ThenInclude(d => d.Verifier)
                .Where(p => !p.IsApproved && (p.Status == "Pending" || p.Status == "Submitted"))
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetApprovedAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Include(p => p.Documents).ThenInclude(d => d.Uploader)
                .Include(p => p.Documents).ThenInclude(d => d.Verifier)
                .Where(p => p.IsApproved)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetForSaleAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Include(p => p.Documents).ThenInclude(d => d.Uploader)
                .Include(p => p.Documents).ThenInclude(d => d.Verifier)
                .Where(p => p.IsAvailable && p.IsApproved && (p.Status == "Listed for Sale" || p.Status == "Approved"))
                .OrderByDescending(p => p.SaleListedDate ?? p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetAvailableAsync()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Include(p => p.Documents).ThenInclude(d => d.Uploader)
                .Include(p => p.Documents).ThenInclude(d => d.Verifier)
                .Where(p => p.IsApproved && p.IsAvailable)
                .OrderByDescending(p => p.SaleListedDate ?? p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> SearchAsync(
            string? query,
            string? status,
            string? city,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var properties = _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Agent)
                .Include(p => p.Documents).ThenInclude(d => d.Uploader)
                .Include(p => p.Documents).ThenInclude(d => d.Verifier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = status.Trim().ToLower();
                properties = properties.Where(p => (p.Status ?? string.Empty).ToLower().Contains(normalizedStatus));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var trimmedCity = city.Trim();
                properties = properties.Where(p => EF.Functions.Like(p.City, $"%{trimmedCity}%"));
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var trimmedQuery = query.Trim();
                properties = properties.Where(p =>
                    EF.Functions.Like(p.Title, $"%{trimmedQuery}%") ||
                    EF.Functions.Like(p.PropertyNumber, $"%{trimmedQuery}%") ||
                    EF.Functions.Like(p.Address, $"%{trimmedQuery}%"));
            }

            if (minPrice.HasValue)
                properties = properties.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                properties = properties.Where(p => p.Price <= maxPrice.Value);

            return await properties
                .OrderByDescending(p => p.CreatedAt)
                .Take(200)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> PropertyNumberExistsAsync(string propertyNumber)
        {
            return await _context.Properties.AnyAsync(p => p.PropertyNumber == propertyNumber);
        }

        public async Task AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
        }

        public Task UpdateAsync(Property property)
        {
            _context.Properties.Update(property);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update failed: {ex.Message}");
                return false;
            }
        }
    }
}
