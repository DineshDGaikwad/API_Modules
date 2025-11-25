using APIPropertyRegistry.Data;
using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Implementations
{
    public class AgentPropertyService : IAgentPropertyService
    {
        private readonly IAgentPropertyRepository _repository;
        private readonly ApplicationDbContext _context;

        public AgentPropertyService(IAgentPropertyRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<AgentPropertyResponseDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToResponse);
        }

        public async Task<IEnumerable<AgentPropertyResponseDto>> GetByAgentAsync(int agentId)
        {
            var list = await _repository.GetByAgentAsync(agentId);
            return list.Select(MapToResponse);
        }

        public async Task<AgentPropertyResponseDto?> GetByIdAsync(int id)
        {
            var ap = await _repository.GetByIdAsync(id);
            return ap == null ? null : MapToResponse(ap);
        }

        public async Task<AgentPropertyResponseDto> CreateAsync(AgentPropertyCreateDto dto)
        {
            var agent = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.AgentId &&
                                          u.Role.Equals("Agent", StringComparison.OrdinalIgnoreCase) &&
                                          u.IsApproved);

            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.PropertyId == dto.PropertyId && p.IsApproved);

            if (agent == null)
                throw new InvalidOperationException("Invalid or unapproved agent.");

            if (property == null)
                throw new InvalidOperationException("Invalid or unapproved property.");

            if (await _repository.GetActiveAgentIdByPropertyAsync(dto.PropertyId) != null)
                throw new InvalidOperationException("This property is already assigned to an active agent.");

            var newAssignment = new AgentProperty
            {
                AgentId = dto.AgentId,
                PropertyId = dto.PropertyId,
                AssignedDate = DateTime.UtcNow,
                Status = "Pending",
                Notes = dto.Notes
            };

            var created = await _repository.AddAsync(newAssignment);
            return MapToResponse(created);
        }

        public async Task<bool> ApproveAsync(AgentPropertyApproveDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var ap = await _repository.GetByIdAsync(dto.AgentPropertyId);
                if (ap == null)
                    return false;

                ap.IsApproved = dto.Approve;
                ap.Status = dto.Approve ? "Active" : "Revoked";
                ap.AssignedDate = DateTime.UtcNow;

                await _repository.UpdateAsync(ap);

                var property = await _context.Properties.FindAsync(ap.PropertyId);
                if (property != null)
                {
                    property.AgentId = dto.Approve ? ap.AgentId : null;
                    _context.Properties.Update(property);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static AgentPropertyResponseDto MapToResponse(AgentProperty ap)
        {
            return new AgentPropertyResponseDto
            {
                AgentPropertyId = ap.AgentPropertyId,
                AgentId = ap.AgentId,
                AgentName = ap.Agent?.FullName ?? "Unknown",
                PropertyId = ap.PropertyId,
                PropertyTitle = ap.Property?.Title ?? "Unknown",
                Status = ap.Status,
                IsApproved = ap.IsApproved,
                AssignedDate = ap.AssignedDate,
                Notes = ap.Notes
            };
        }
    }
}
