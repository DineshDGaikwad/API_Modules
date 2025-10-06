namespace APIJobPortal.DTOs.Company
{
    public class CreateCompanyDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Website { get; set; }
    }
}
