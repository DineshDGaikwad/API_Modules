using APIPropertyRegistry.Data;
using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace APIPropertyRegistry.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan AdminCacheTtl = TimeSpan.FromSeconds(30);

        public DashboardService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<AdminDashboardSummaryDto> GetAdminDashboardAsync(DateTime? from = null, DateTime? to = null)
        {
            var cacheKey = $"admin_summary_{from:O}_{to:O}";

            if (_cache.TryGetValue(cacheKey, out AdminDashboardSummaryDto? cached) && cached is not null)
                return cached;

            var totalUsers = await _context.Users.CountAsync();
            var totalAgents = await _context.Users.CountAsync(u => EF.Functions.Like(u.Role, "Agent") || EF.Functions.Like(u.Role, "agent"));
            var totalProperties = await _context.Properties.CountAsync();
            var totalTransactions = await _context.PropertyTransactions.CountAsync();
            var totalRevenue = await _context.PropertyTransactions.SumAsync(t => (decimal?)t.Amount) ?? 0m;
            var pendingVerifications = await _context.Documents.CountAsync(d => !d.Verified);

            DateTime fromDate = from ?? DateTime.UtcNow.AddDays(-30);
            DateTime toDate = to ?? DateTime.UtcNow;

            var monthlyRevenue = await _context.PropertyTransactions
                .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;

            var topAgents = await _context.PropertyTransactions
                .Where(t => t.AgentId != null && t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .GroupBy(t => t.AgentId)
                .Select(g => new
                {
                    AgentId = g.Key,
                    SalesCount = g.Count(),
                    TotalCommission = g.Sum(x => (decimal?)(x.AgentCommission ?? 0m)) ?? 0m
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(5)
                .ToListAsync();

            var topAgentsDto = topAgents
                .Select(a => new TopAgentDto
                {
                    AgentId = a.AgentId ?? 0,
                    AgentName = _context.Users.Where(u => u.UserId == (a.AgentId ?? 0))
                                              .Select(u => u.FullName)
                                              .FirstOrDefault() ?? "Unknown",
                    SalesCount = a.SalesCount,
                    TotalCommission = a.TotalCommission
                }).ToList();

            var result = new AdminDashboardSummaryDto
            {
                TotalUsers = totalUsers,
                TotalAgents = totalAgents,
                TotalProperties = totalProperties,
                TotalTransactions = totalTransactions,
                TotalRevenue = totalRevenue,
                PendingVerifications = pendingVerifications,
                MonthlyRevenue = monthlyRevenue,
                TopAgents = topAgentsDto
            };

            _cache.Set(cacheKey, result, AdminCacheTtl);
            return result;
        }


        public async Task<AgentDashboardDto> GetAgentDashboardAsync(int agentId, DateTime? from = null, DateTime? to = null)
        {
            DateTime fromDate = from ?? DateTime.UtcNow.AddDays(-30);
            DateTime toDate = to ?? DateTime.UtcNow;

            var assignedProps = await _context.AgentProperties.CountAsync(a => a.AgentId == agentId);
            var activeProps = await _context.AgentProperties.CountAsync(a => a.AgentId == agentId && a.Status == "Active");
            var verifiedDocs = await _context.Documents.CountAsync(d => d.Verified && d.VerifiedBy == agentId);
            var totalSales = await _context.PropertyTransactions.CountAsync(t => t.AgentId == agentId && t.TransactionDate >= fromDate && t.TransactionDate <= toDate);
            var totalCommission = await _context.PropertyTransactions
                .Where(t => t.AgentId == agentId && t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .SumAsync(t => (decimal?)(t.AgentCommission ?? 0m)) ?? 0m;

            return new AgentDashboardDto
            {
                AgentId = agentId,
                AssignedProperties = assignedProps,
                ActiveProperties = activeProps,
                VerifiedDocuments = verifiedDocs,
                TotalSales = totalSales,
                TotalCommission = totalCommission
            };
        }

        public async Task<UserDashboardDto> GetUserDashboardAsync(int userId, DateTime? from = null, DateTime? to = null)
        {
            DateTime fromDate = from ?? DateTime.UtcNow.AddDays(-30);
            DateTime toDate = to ?? DateTime.UtcNow;

            var ownedProps = await _context.PropertyOwnerships.CountAsync(p => p.UserId == userId);
            var transactions = await _context.PropertyTransactions.CountAsync(t => t.BuyerId == userId && t.TransactionDate >= fromDate && t.TransactionDate <= toDate);
            var totalSpent = await _context.PropertyTransactions
                .Where(t => t.BuyerId == userId && t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;
            var uploadedDocs = await _context.Documents.CountAsync(d => d.UploadedBy == userId);
            var pending = await _context.Documents.CountAsync(d => d.UploadedBy == userId && !d.Verified);

            return new UserDashboardDto
            {
                UserId = userId,
                OwnedProperties = ownedProps,
                TotalTransactions = transactions,
                TotalSpent = totalSpent,
                UploadedDocuments = uploadedDocs,
                PendingVerifications = pending
            };
        }
    }
}
