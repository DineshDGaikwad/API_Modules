using System;
using System.ComponentModel.DataAnnotations;

namespace APIPropertyRegistry.Models
{
public class PropertyOwnership
{
[Key]
public int OwnershipId { get; set; }
public int PropertyId { get; set; }
public int UserId { get; set; }
public string OwnershipType { get; set; } = "Primary";
public bool Verified { get; set; } = false;
public int? VerifiedBy { get; set; }
public DateTime? VerifiedDate { get; set; }
public DateTime StartDate { get; set; } = DateTime.UtcNow;
public DateTime? EndDate { get; set; }
public string Status { get; set; } = "Pending";

public Property? Property { get; set; }
public User? User { get; set; }
public User? Verifier { get; set; }
}
}

