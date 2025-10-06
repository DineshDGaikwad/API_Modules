using System.ComponentModel.DataAnnotations;

namespace APIJobPortal.Models
{
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public string? Type { get; set; } // Full-Time, Part-Time, Remote

        public decimal Salary { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(100)]
        public string? RequiredExperience { get; set; }

        public List<JobApplication> JobApplications { get; set; } = new(); 
        public List<CompanyJob> CompanyJobs { get; set; } = new();         

    }
}
