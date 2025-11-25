using System;
using System.Collections.Generic;

namespace APIPropertyRegistry.DTOs
{
    public class AgentApprovalResponseDto
    {
        public int AgentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string Status => IsApproved ? "Approved" : "Pending";
        public DateTime CreatedAt { get; set; }
    }

    public class ApproveAgentDto
    {
        public bool Approve { get; set; }
        public string? Remarks { get; set; }
    }

    public class PropertyApprovalResponseDto
    {
        public int PropertyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string Status => IsApproved ? "Approved" : "Pending";
        public DateTime CreatedAt { get; set; }
    }

    public class ApprovePropertyDto
    {
        public bool Approve { get; set; }
        public string? Remarks { get; set; }
    }

    public class AdminUserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AdminAgentDto
    {
        public int AgentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string Status => IsApproved ? "Approved" : "Pending";
        public int PropertiesCount { get; set; }
        public int TransactionsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminPropertyDto
    {
        public int PropertyId { get; set; }
        public string PropertyNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string? AgentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }

    public class AdminTransactionDto
    {
        public int TransactionId { get; set; }
        public string PropertyNumber { get; set; } = string.Empty;
        public string PropertyTitle { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public string? AgentName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? AgentCommission { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class AdminSearchResultDto
    {
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? SecondaryInfo { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class UpdatePropertyDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string City { get; set; } = string.Empty;
    }
}
