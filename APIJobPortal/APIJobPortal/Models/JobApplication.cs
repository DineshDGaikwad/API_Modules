using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIJobPortal.Models
{
    public class JobApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public int JobSeekerId { get; set; }

        [Required]
        public int JobId { get; set; }

        [MaxLength(50)]
        public string ApplicationStatus { get; set; } = "Pending";  // Pending / Shortlisted / Rejected

        public string? ResumeLink { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

        public string? CoverLetter { get; set; }

        public JobSeeker JobSeeker { get; set; } = null!;
        public Job Job { get; set; } = null!;
    }
}
