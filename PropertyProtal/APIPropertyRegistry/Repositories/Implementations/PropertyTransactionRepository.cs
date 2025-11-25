using APIPropertyRegistry.Data;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Implementations
{
    public class PropertyTransactionRepository : IPropertyTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<PropertyTransaction> BuildBaseQuery()
        {
            return _context.PropertyTransactions
                .Include(t => t.Property)
                    .ThenInclude(p => p.Documents)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Agent)
                .AsNoTracking();
        }

        public async Task<PropertyTransaction> CreateAsync(PropertyTransaction transaction)
        {
            await _context.PropertyTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<IEnumerable<PropertyTransaction>> GetAllAsync()
        {
            return await BuildBaseQuery()
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<PropertyTransaction?> GetByIdAsync(int id)
        {
            return await BuildBaseQuery()
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }

        public async Task<IEnumerable<PropertyTransaction>> GetByBuyerAsync(int buyerId)
        {
            return await BuildBaseQuery()
                .Where(t => t.BuyerId == buyerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyTransaction>> GetBySellerAsync(int sellerId)
        {
            return await BuildBaseQuery()
                .Where(t => t.SellerId == sellerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyTransaction>> GetByAgentAsync(int agentId, string? status = null, bool includeArchived = false)
        {
            var query = BuildBaseQuery()
                .Where(t => t.AgentId == agentId && t.Status != "Revoked");

            if (!includeArchived)
                query = query.Where(t => !t.IsArchived);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status == status);

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyTransaction>> GetPendingForAdminAsync()
        {
            return await BuildBaseQuery()
                .Where(t => t.Status == "Pending Admin" && !t.IsArchived)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> HasActiveTransactionAsync(int propertyId)
        {
            return await _context.PropertyTransactions
                .AnyAsync(t => t.PropertyId == propertyId && !t.IsArchived && (t.Status == "Pending" || t.Status == "Pending Admin"));
        }

        public async Task<IEnumerable<PropertyTransaction>> GetPendingByPropertyAsync(int propertyId)
        {
            return await BuildBaseQuery()
                .Where(t => t.PropertyId == propertyId && !t.IsArchived && (t.Status == "Pending" || t.Status == "Pending Admin"))
                .ToListAsync();
        }

        public async Task UpdateAsync(PropertyTransaction transaction)
        {
            _context.PropertyTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<PropertyTransaction> transactions)
        {
            _context.PropertyTransactions.UpdateRange(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
