using System;

namespace APIPropertyRegistry.DTOs
{
    public class PropertyTransactionCreateDto
    {
        public int PropertyId { get; set; }
        public int SellerId { get; set; }
        public int BuyerId { get; set; }
        public int? AgentId { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal? AgentCommission { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    public class PropertyTransactionVerifyDto
    {
        public int TransactionId { get; set; }
        public int VerifierId { get; set; }
        public bool Approve { get; set; }
    }

    public class AgentTransactionDecisionDto
    {
        public int TransactionId { get; set; }
        public int AgentId { get; set; }
        public bool Approve { get; set; }
        public string? Remarks { get; set; }
    }

    public class AdminTransactionDecisionDto
    {
        public int TransactionId { get; set; }
        public int AdminId { get; set; }
        public bool Approve { get; set; }
        public string? Remarks { get; set; }
    }

    public class PropertyTransactionResponseDto
    {
        public int TransactionId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyNumber { get; set; } = string.Empty;
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public string PropertyCity { get; set; } = string.Empty;
        public decimal PropertyPrice { get; set; }
        public string? PropertyImageUrl { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public int? AgentId { get; set; }
        public string? AgentName { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal? AgentCommission { get; set; }
        public string Status { get; set; } = "Pending";
        public string Stage { get; set; } = "AgentReview";
        public bool IsArchived { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime? AgentDecisionDate { get; set; }
        public DateTime? AdminDecisionDate { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string? AgentRemarks { get; set; }
        public string? AdminRemarks { get; set; }
    }
}
