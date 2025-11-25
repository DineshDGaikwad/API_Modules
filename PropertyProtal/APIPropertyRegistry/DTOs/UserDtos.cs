using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace APIPropertyRegistry.DTOs
{
    public class UserCreateDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        [MinLength(4)]
        public required string Password { get; set; }
        public required string MobileNumber { get; set; }
        public required string Role { get; set; }
    }

    public class UserUpdateDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public bool? IsApproved { get; set; }
    }

    public class UserLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class UserResponseDto
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string MobileNumber { get; set; }
        public required string Role { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Remarks { get; set; }
        public string? ProfileImageUrl { get; set; }
    }

    public class AgentApprovalDto
    {
        public int AgentId { get; set; }
        public bool Approve { get; set; }
        public int? AdminId { get; set; }
        public string? Remarks { get; set; }
    }
    
    public class UserProfileResponseDto
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Role { get; set; } = "User";
    }

    public class UserProfileUpdateDto
    {
        public string? FullName { get; set; }
        public string? MobileNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
