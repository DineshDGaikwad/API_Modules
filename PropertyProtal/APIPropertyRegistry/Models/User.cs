using System;
using System.Collections.Generic;

namespace APIPropertyRegistry.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsApproved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Remarks { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImageUrl { get; set; }
        public User? Approver { get; set; }
        public ICollection<Property>? CreatedProperties { get; set; }
        public ICollection<Document>? UploadedDocuments { get; set; }
        public ICollection<PropertyOwnership>? OwnedProperties { get; set; }
        public ICollection<PropertyTransaction>? BuyerTransactions { get; set; }
        public ICollection<PropertyTransaction>? SellerTransactions { get; set; }
        public ICollection<PropertyTransaction>? AgentTransactions { get; set; }
        public ICollection<Property>? OwnedPropertyListings { get; set; }
        public ICollection<Property>? AgentPropertyListings { get; set; }
        public ICollection<Property>? VerifiedProperties { get; set; }
    }
}
