using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Interfaces
{
    public interface IAgentPropertyRepository
    {
        Task<AgentProperty?> GetByIdAsync(int id);
        Task<IEnumerable<AgentProperty>> GetAllAsync();
        Task<IEnumerable<AgentProperty>> GetByAgentAsync(int agentId);
        Task<IEnumerable<AgentProperty>> GetByPropertyAsync(int propertyId, bool track = false);
        Task<AgentProperty> AddAsync(AgentProperty agentProperty);
        Task<bool> UpdateAsync(AgentProperty agentProperty);
        Task<bool> DeleteAsync(int id);
        Task<int?> GetActiveAgentIdByPropertyAsync(int propertyId);
    }
}
