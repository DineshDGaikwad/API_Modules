using APIPropertyRegistry.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IAgentPropertyService
    {
        Task<IEnumerable<AgentPropertyResponseDto>> GetAllAsync();
        Task<IEnumerable<AgentPropertyResponseDto>> GetByAgentAsync(int agentId);
        Task<AgentPropertyResponseDto?> GetByIdAsync(int id);
        Task<AgentPropertyResponseDto> CreateAsync(AgentPropertyCreateDto dto);
        Task<bool> ApproveAsync(AgentPropertyApproveDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
