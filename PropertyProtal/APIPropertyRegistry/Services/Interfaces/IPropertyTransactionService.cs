using APIPropertyRegistry.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IPropertyTransactionService
    {
        Task<PropertyTransactionResponseDto> CreateTransactionAsync(PropertyTransactionCreateDto dto);
        Task<IEnumerable<PropertyTransactionResponseDto>> GetAllAsync();
        Task<PropertyTransactionResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PropertyTransactionResponseDto>> GetByBuyerAsync(int buyerId);
        Task<IEnumerable<PropertyTransactionResponseDto>> GetBySellerAsync(int sellerId);
        Task<IEnumerable<PropertyTransactionResponseDto>> GetByAgentAsync(int agentId, string? status = null, bool includeArchived = false);
        Task<IEnumerable<PropertyTransactionResponseDto>> GetPendingForAdminAsync();
        Task<bool> SubmitAgentDecisionAsync(AgentTransactionDecisionDto dto);
        Task<bool> SubmitAdminDecisionAsync(AdminTransactionDecisionDto dto);
        Task<bool> VerifyTransactionAsync(PropertyTransactionVerifyDto dto);
    }
}
