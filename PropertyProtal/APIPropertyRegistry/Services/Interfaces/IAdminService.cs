using APIPropertyRegistry.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AgentApprovalResponseDto>> GetPendingAgentsAsync();
        Task<IEnumerable<AgentApprovalResponseDto>> GetApprovedAgentsAsync();
        Task<bool> ApproveOrRejectAgentAsync(int agentId, bool approve, string? remarks);

        Task<IEnumerable<PropertyApprovalResponseDto>> GetPendingPropertiesAsync();
        Task<IEnumerable<PropertyApprovalResponseDto>> GetApprovedPropertiesAsync();
        Task<bool> ApproveOrRejectPropertyAsync(int propertyId, int adminId, bool approve, string? remarks);

        Task<IEnumerable<AdminUserDto>> GetAllUsersAsync();
        Task<IEnumerable<AdminAgentDto>> GetAllAgentsAsync();
        Task<AdminUserDto?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);

        Task<IEnumerable<AdminPropertyDto>> GetAllPropertiesAsync();
        Task<bool> DeletePropertyAsync(int id);

        Task<IEnumerable<AdminTransactionDto>> GetRecentTransactionsAsync(int limit = 20);

        Task<IEnumerable<AdminSearchResultDto>> SearchAsync(string query, string type);
    }
}
