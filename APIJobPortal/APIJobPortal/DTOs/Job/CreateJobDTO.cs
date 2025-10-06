namespace APIJobPortal.DTOs.Job
{
    public class CreateJobDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Type { get; set; }
        public decimal Salary { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? RequiredExperience { get; set; }
    }
}
