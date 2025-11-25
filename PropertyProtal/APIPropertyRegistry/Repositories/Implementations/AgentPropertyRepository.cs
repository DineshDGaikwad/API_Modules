using APIPropertyRegistry.Data;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Implementations
{
    public class AgentPropertyRepository : IAgentPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public AgentPropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AgentProperty?> GetByIdAsync(int id)
        {
            return await _context.AgentProperties
                .Include(ap => ap.Agent)
                .Include(ap => ap.Property)
                .AsNoTracking()
                .FirstOrDefaultAsync(ap => ap.AgentPropertyId == id);
        }

        public async Task<IEnumerable<AgentProperty>> GetAllAsync()
        {
            return await _context.AgentProperties
                .Include(ap => ap.Agent)
                .Include(ap => ap.Property)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AgentProperty>> GetByAgentAsync(int agentId)
        {
            return await _context.AgentProperties
                .Include(ap => ap.Property)
                .Where(ap => ap.AgentId == agentId && ap.Status != "Revoked")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AgentProperty>> GetByPropertyAsync(int propertyId, bool track = false)
        {
            var query = _context.AgentProperties
                .Where(ap => ap.PropertyId == propertyId);

            return track
                ? await query.AsTracking().ToListAsync()
                : await query.AsNoTracking().ToListAsync();
        }

        public async Task<AgentProperty> AddAsync(AgentProperty agentProperty)
        {
            await _context.AgentProperties.AddAsync(agentProperty);
            await _context.SaveChangesAsync();
            return agentProperty;
        }

        public async Task<bool> UpdateAsync(AgentProperty agentProperty)
        {
            var existing = await _context.AgentProperties
                .FirstOrDefaultAsync(ap => ap.AgentPropertyId == agentProperty.AgentPropertyId);

            if (existing == null)
                return false;

            existing.Status = agentProperty.Status;
            existing.IsApproved = agentProperty.IsApproved;
            existing.AssignedDate = agentProperty.AssignedDate;
            existing.Notes = agentProperty.Notes;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.AgentProperties.FindAsync(id);
            if (existing == null) return false;

            _context.AgentProperties.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int?> GetActiveAgentIdByPropertyAsync(int propertyId)
        {
            return await _context.AgentProperties
                .Where(ap => ap.PropertyId == propertyId && ap.Status == "Active" && ap.IsApproved)
                .OrderByDescending(ap => ap.AssignedDate)
                .Select(ap => (int?)ap.AgentId)
                .FirstOrDefaultAsync();
        }
    }
}
