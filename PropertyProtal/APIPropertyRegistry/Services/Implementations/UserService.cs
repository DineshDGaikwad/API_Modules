using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using APIPropertyRegistry.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace APIPropertyRegistry.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly Cloudinary _cloudinary;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;

            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<bool> RegisterUserAsync(UserCreateDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return false;

            var role = NormalizeRole(dto.Role);
            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                MobileNumber = dto.MobileNumber,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                IsApproved = !role.Equals("Agent", StringComparison.OrdinalIgnoreCase)
            };

            await _userRepository.AddUserAsync(newUser);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<User?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || user.Password != dto.Password)
                return null;

            if (user.Role.Equals("Agent", StringComparison.OrdinalIgnoreCase) && !user.IsApproved)
                return null;

            return user;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdNoTrackingAsync(id);
            return user == null ? null : MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user == null ? null : MapToResponseDto(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return Enumerable.Empty<UserResponseDto>();

            var users = await _userRepository.GetUsersByRoleAsync(role);
            return users.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<UserResponseDto>> GetPendingAgentsAsync()
        {
            var users = await _userRepository.GetPendingAgentsAsync();
            return users.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<UserResponseDto>> SearchUsersAsync(string query, string? role = null)
        {
            var users = await _userRepository.SearchAsync(query ?? string.Empty, role);
            return users.Select(MapToResponseDto);
        }

        public async Task<bool> ApproveAgentAsync(int agentId, bool approve, int adminId, string? remarks)
        {
            var admin = await _userRepository.GetUserByIdNoTrackingAsync(adminId);
            if (admin == null || !admin.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return false;

            var agent = await _userRepository.GetUserByIdAsync(agentId);
            if (agent == null || !agent.Role.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                return false;

            agent.IsApproved = approve;
            agent.ApprovedBy = adminId;
            agent.ApprovedDate = DateTime.UtcNow;
            agent.Remarks = remarks;

            if (approve)
                agent.Role = "Agent";

            await _userRepository.UpdateUserAsync(agent);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return false;

            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = dto.Password;
            if (!string.IsNullOrEmpty(dto.Role))
                user.Role = NormalizeRole(dto.Role);
            if (dto.IsApproved.HasValue)
                user.IsApproved = dto.IsApproved.Value;

            await _userRepository.UpdateUserAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<UserResponseDto?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdNoTrackingAsync(userId);
            return user == null ? null : MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateProfileAsync(int userId, IFormFile? profileImage, string? fullName, string? mobileNumber)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            if (!string.IsNullOrWhiteSpace(fullName))
                user.FullName = fullName;

            if (!string.IsNullOrWhiteSpace(mobileNumber))
                user.MobileNumber = mobileNumber;

            if (profileImage != null)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(profileImage.FileName, profileImage.OpenReadStream()),
                    Folder = "profile_photos"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                    throw new Exception($"Cloudinary Error: {uploadResult.Error.Message}");

                user.ProfileImageUrl = uploadResult.SecureUrl.ToString();
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToResponseDto(user);
        }

        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsApproved = user.IsApproved,
                ApprovedDate = user.ApprovedDate,
                Remarks = user.Remarks,
                ProfileImageUrl = user.ProfileImageUrl
            };
        }

        private static string NormalizeRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return "User";

            if (role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return "Admin";

            if (role.Equals("agent", StringComparison.OrdinalIgnoreCase))
                return "Agent";

            return "User";
        }
    }
}
