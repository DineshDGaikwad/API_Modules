using System;

namespace APIPropertyRegistry.DTOs
{
    public class AgentPropertyCreateDto
    {
        public int AgentId { get; set; }
        public int PropertyId { get; set; }
        public string? Notes { get; set; }
    }

    public class AgentPropertyApproveDto
    {
        public int AgentPropertyId { get; set; }
        public bool Approve { get; set; }
    }

    public class AgentPropertyResponseDto
    {
        public int AgentPropertyId { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public bool IsApproved { get; set; }
        public DateTime AssignedDate { get; set; }
        public string? Notes { get; set; }
    }
}
