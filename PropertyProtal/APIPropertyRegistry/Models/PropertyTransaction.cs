using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIPropertyRegistry.Models
{
public class PropertyTransaction
{
[Key]
public int TransactionId { get; set; }
[Required, ForeignKey("Property")] public int PropertyId { get; set; }
[Required, ForeignKey("Seller")] public int SellerId { get; set; }
[Required, ForeignKey("Buyer")] public int BuyerId { get; set; }
[ForeignKey("Agent")] public int? AgentId { get; set; }
[Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
[Column(TypeName = "decimal(18,2)")] public decimal? AgentCommission { get; set; }
[StringLength(50)] public string Status { get; set; } = "Pending";
[StringLength(50)] public string Stage { get; set; } = "AgentReview";
public int? VerifiedBy { get; set; }
public DateTime? VerifiedDate { get; set; }
public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
public DateTime? AgentDecisionDate { get; set; }
public DateTime? AdminDecisionDate { get; set; }
[StringLength(250)] public string? AgentRemarks { get; set; }
[StringLength(250)] public string? AdminRemarks { get; set; }
public bool IsArchived { get; set; }
public DateTime? ArchivedDate { get; set; }
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? UpdatedAt { get; set; }

public Property? Property { get; set; }
public User? Seller { get; set; }
public User? Buyer { get; set; }
public User? Agent { get; set; }
public User? Verifier { get; set; }
}
}
