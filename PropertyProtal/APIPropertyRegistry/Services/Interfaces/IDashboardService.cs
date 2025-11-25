using APIPropertyRegistry.DTOs;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardSummaryDto> GetAdminDashboardAsync(DateTime? from = null, DateTime? to = null);
        Task<AgentDashboardDto> GetAgentDashboardAsync(int agentId, DateTime? from = null, DateTime? to = null);
        Task<UserDashboardDto> GetUserDashboardAsync(int userId, DateTime? from = null, DateTime? to = null);
    }
}
