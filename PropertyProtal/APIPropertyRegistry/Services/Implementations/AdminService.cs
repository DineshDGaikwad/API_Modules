using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        private readonly IPropertyRepository _propertyRepository;
        private readonly PropertyNumberGeneratorService _propertyNumberGenerator;

        public AdminService(IAdminRepository repo, IPropertyRepository propertyRepository, PropertyNumberGeneratorService propertyNumberGenerator)
        {
            _repo = repo;
            _propertyRepository = propertyRepository;
            _propertyNumberGenerator = propertyNumberGenerator;
        }

        public async Task<IEnumerable<AgentApprovalResponseDto>> GetPendingAgentsAsync()
        {
            var agents = await _repo.GetPendingAgentsAsync();
            return agents.Select(a => new AgentApprovalResponseDto
            {
                AgentId = a.UserId,
                FullName = a.FullName,
                Email = a.Email,
                MobileNumber = a.MobileNumber,
                IsApproved = a.IsApproved,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<IEnumerable<AgentApprovalResponseDto>> GetApprovedAgentsAsync()
        {
            var agents = await _repo.GetApprovedAgentsAsync();
            return agents.Select(a => new AgentApprovalResponseDto
            {
                AgentId = a.UserId,
                FullName = a.FullName,
                Email = a.Email,
                MobileNumber = a.MobileNumber,
                IsApproved = a.IsApproved,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<bool> ApproveOrRejectAgentAsync(int agentId, bool approve, string? remarks)
        {
            var agent = await _repo.GetAgentByIdAsync(agentId);
            if (agent == null) return false;

            agent.IsApproved = approve;
            agent.Remarks = remarks;
            agent.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAgentAsync(agent);
            return true;
        }

        public async Task<IEnumerable<PropertyApprovalResponseDto>> GetPendingPropertiesAsync()
        {
            var properties = await _repo.GetPendingPropertiesAsync();
            return properties.Select(p => new PropertyApprovalResponseDto
            {
                PropertyId = p.PropertyId,
                Title = p.Title,
                Address = p.Address,
                City = p.City,
                OwnerName = p.Owner?.FullName ?? "Unknown",
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt
            });
        }

        public async Task<IEnumerable<PropertyApprovalResponseDto>> GetApprovedPropertiesAsync()
        {
            var properties = await _repo.GetApprovedPropertiesAsync();
            return properties.Select(p => new PropertyApprovalResponseDto
            {
                PropertyId = p.PropertyId,
                Title = p.Title,
                Address = p.Address,
                City = p.City,
                OwnerName = p.Owner?.FullName ?? "Unknown",
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt
            });
        }

        public async Task<bool> ApproveOrRejectPropertyAsync(int propertyId, int adminId, bool approve, string? remarks)
        {
            var property = await _repo.GetPropertyByIdAsync(propertyId);
            if (property == null) return false;

            property.IsApproved = approve;
            property.ApprovedBy = adminId;
            property.ApprovedDate = DateTime.UtcNow;
            property.Remarks = remarks;
            property.Status = approve ? "Approved" : "Rejected";
            property.UpdatedAt = DateTime.UtcNow;

            if (approve && string.IsNullOrEmpty(property.PropertyNumber))
            {
                property.PropertyNumber = await GenerateUniquePropertyNumberAsync();
            }

            await _repo.UpdatePropertyAsync(property);
            return true;
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllUsersAsync("User");
            return users.Select(u => new AdminUserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.MobileNumber,
                Role = u.Role,
                IsApproved = u.IsApproved,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
        }

        public async Task<IEnumerable<AdminAgentDto>> GetAllAgentsAsync()
        {
            var agents = await _repo.GetAllAgentsAsync();
            return agents.Select(a => new AdminAgentDto
            {
                AgentId = a.UserId,
                FullName = a.FullName,
                Email = a.Email,
                Phone = a.MobileNumber,
                IsApproved = a.IsApproved,
                PropertiesCount = 0,
                TransactionsCount = 0,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<AdminUserDto?> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return null;

            return new AdminUserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.MobileNumber,
                Role = user.Role,
                IsApproved = user.IsApproved,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return false;

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.MobileNumber = dto.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _repo.DeleteUserAsync(id);
        }

        public async Task<IEnumerable<AdminPropertyDto>> GetAllPropertiesAsync()
        {
            var properties = await _repo.GetAllPropertiesAsync();
            return properties.Select(p => new AdminPropertyDto
            {
                PropertyId = p.PropertyId,
                PropertyNumber = p.PropertyNumber,
                Title = p.Title,
                Address = p.Address,
                City = p.City,
                Price = p.Price,
                Area = p.Area,
                Status = p.Status,
                IsApproved = p.IsApproved,
                OwnerName = p.Owner?.FullName ?? "Unknown",
                AgentName = p.Agent?.FullName,
                CreatedAt = p.CreatedAt,
                ApprovedDate = p.ApprovedDate
            });
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            return await _repo.DeletePropertyAsync(id);
        }

        public async Task<IEnumerable<AdminTransactionDto>> GetRecentTransactionsAsync(int limit = 20)
        {
            var transactions = await _repo.GetRecentTransactionsAsync(limit);
            return transactions.Select(t => new AdminTransactionDto
            {
                TransactionId = t.TransactionId,
                PropertyNumber = t.Property?.PropertyNumber ?? "N/A",
                PropertyTitle = t.Property?.Title ?? "Unknown",
                BuyerName = t.Buyer?.FullName ?? "Unknown",
                SellerName = t.Seller?.FullName ?? "Unknown",
                AgentName = t.Agent?.FullName,
                Amount = t.Amount,
                Status = t.Status ?? "Completed",
                AgentCommission = t.AgentCommission,
                TransactionDate = t.TransactionDate
            });
        }

        public async Task<IEnumerable<AdminSearchResultDto>> SearchAsync(string query, string type)
        {
            if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<AdminSearchResultDto>();

            var results = new List<AdminSearchResultDto>();

            if (type == "users" || type == "all")
            {
                var users = await _repo.SearchUsersAsync(query);
                results.AddRange(users.Select(u => new AdminSearchResultDto
                {
                    Type = "User",
                    Id = u.UserId,
                    Title = u.FullName,
                    Email = u.Email,
                    SecondaryInfo = u.MobileNumber,
                    CreatedAt = u.CreatedAt
                }));
            }

            if (type == "agents" || type == "all")
            {
                var agents = await _repo.SearchAgentsAsync(query);
                results.AddRange(agents.Select(a => new AdminSearchResultDto
                {
                    Type = "Agent",
                    Id = a.UserId,
                    Title = a.FullName,
                    Email = a.Email,
                    SecondaryInfo = a.MobileNumber,
                    CreatedAt = a.CreatedAt
                }));
            }

            if (type == "properties" || type == "all")
            {
                var properties = await _repo.SearchPropertiesAsync(query);
                results.AddRange(properties.Select(p => new AdminSearchResultDto
                {
                    Type = "Property",
                    Id = p.PropertyId,
                    Title = p.Title,
                    Email = null,
                    SecondaryInfo = p.Address,
                    CreatedAt = p.CreatedAt
                }));
            }

            return results.OrderByDescending(r => r.CreatedAt);
        }

        private async Task<string> GenerateUniquePropertyNumberAsync()
        {
            string propertyNumber;
            do
            {
                propertyNumber = _propertyNumberGenerator.GeneratePropertyNumber();
            }
            while (await _propertyRepository.PropertyNumberExistsAsync(propertyNumber));

            return propertyNumber;
        }
    }
}
