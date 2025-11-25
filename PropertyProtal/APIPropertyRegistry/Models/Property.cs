using System;
using System.Collections.Generic;

namespace APIPropertyRegistry.Models
{
public class Property
{
public int PropertyId { get; set; }
public string PropertyNumber { get; set; } = string.Empty;
public string Title { get; set; } = string.Empty;
public string Description { get; set; } = string.Empty;
public string Address { get; set; } = string.Empty;
public string City { get; set; } = string.Empty;
public decimal Area { get; set; }
public decimal Price { get; set; }
public bool IsApproved { get; set; } = false;
public bool IsAvailable { get; set; } = true;
public bool IsForSale { get; set; } = false;
public int? ApprovedBy { get; set; }
public DateTime? ApprovedDate { get; set; }
public DateTime? SaleListedDate { get; set; }
public string Status { get; set; } = "Pending";
public string? Remarks { get; set; }
public int CreatedBy { get; set; }
public int? OwnerId { get; set; }
public int? AgentId { get; set; }
public int? VerifiedBy { get; set; }
public DateTime? VerifiedDate { get; set; }
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? UpdatedAt { get; set; }

public User? Creator { get; set; }
public User? Owner { get; set; }
public User? Agent { get; set; }
public User? Verifier { get; set; }
public User? Approver { get; set; }

public ICollection<Document> Documents { get; set; } = new List<Document>();
public ICollection<PropertyOwnership>? Ownerships { get; set; }
public ICollection<PropertyTransaction>? Transactions { get; set; }
}
}
