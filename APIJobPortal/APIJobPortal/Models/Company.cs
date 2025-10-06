using System.ComponentModel.DataAnnotations;


namespace APIJobPortal.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Industry { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(15)]
        public string? ContactNumber { get; set; }

        [Url]
        public string? Website { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<JobApplication> JobApplications { get; set; } = new(); 
        public List<CompanyJob> CompanyJobs { get; set; } = new();          

    }
}
