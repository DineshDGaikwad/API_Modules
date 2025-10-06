namespace APIJobPortal.DTOs.Company
{
    public class GetCompanyDTO
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
