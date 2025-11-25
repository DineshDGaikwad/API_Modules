using System.Collections.Generic;
using System.Threading.Tasks;
using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Models;


namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserCreateDto dto);
        Task<User?> LoginAsync(UserLoginDto dto);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<UserResponseDto>> GetPendingAgentsAsync();
        Task<IEnumerable<UserResponseDto>> SearchUsersAsync(string query, string? role = null);
        Task<bool> ApproveAgentAsync(int agentId, bool approve, int adminId, string? remarks);
        Task<bool> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserResponseDto?> GetProfileAsync(int userId);
        Task<UserResponseDto?> UpdateProfileAsync(int userId, IFormFile? profileImage, string? fullName, string? mobileNumber);

    }
}
