using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIJobPortal.Models
{
    public class JobSeeker
    {
        [Key]
        public int JobSeekerId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(200)]
        public string? Skills { get; set; }  // e.g. “C#, SQL, React”

        public int ExperienceYears { get; set; }

        public string? ResumeLink { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

    }
}
