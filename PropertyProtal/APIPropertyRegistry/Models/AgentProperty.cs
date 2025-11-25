using System;

namespace APIPropertyRegistry.Models
{
public class AgentProperty
{
public int AgentPropertyId { get; set; }
public int AgentId { get; set; }
public int PropertyId { get; set; }
public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
public string Status { get; set; } = "Pending";
public bool IsApproved { get; set; } = false;
public string? Notes { get; set; }

public User? Agent { get; set; }
public Property? Property { get; set; }
}
}