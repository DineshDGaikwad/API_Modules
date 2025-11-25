using System;

namespace APIPropertyRegistry.DTOs
{
    public class PropertyOwnershipCreateDto
    {
        public int PropertyId { get; set; }
        public int OwnerId { get; set; }
        public DateTime OwnershipStartDate { get; set; } = DateTime.UtcNow;
    }

    public class PropertyOwnershipVerifyDto
    {
        public int OwnershipId { get; set; }
        public int VerifierId { get; set; }
        public bool Verified { get; set; }
    }

    public class PropertyOwnershipResponseDto
    {
        public int OwnershipId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnershipType { get; set; } = "Primary";
        public bool Verified { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime OwnershipStartDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }

    public class PropertyOwnershipTransferDto
    {
        public int PropertyId { get; set; }
        public int NewOwnerId { get; set; }
        public int VerifierId { get; set; }
        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    }
}
