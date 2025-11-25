namespace APIPropertyRegistry.DTOs
{
    public class TopAgentDto
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
        public decimal TotalCommission { get; set; }
    }

    public class AdminDashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalAgents { get; set; }
        public int TotalProperties { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingVerifications { get; set; }

        public decimal MonthlyRevenue { get; set; }
        public List<TopAgentDto> TopAgents { get; set; } = new();
    }

    public class AgentDashboardDto
    {
        public int AgentId { get; set; }
        public int AssignedProperties { get; set; }
        public int ActiveProperties { get; set; }
        public int VerifiedDocuments { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalCommission { get; set; }
    }

    public class UserDashboardDto
    {
        public int UserId { get; set; }
        public int OwnedProperties { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalSpent { get; set; }
        public int UploadedDocuments { get; set; }
        public int PendingVerifications { get; set; }
    }
}
