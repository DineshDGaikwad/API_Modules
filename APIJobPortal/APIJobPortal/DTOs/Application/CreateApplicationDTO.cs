namespace APIJobPortal.DTOs.Application
{
    public class CreateApplicationDTO
    {
        public int JobSeekerId { get; set; }
        public int JobId { get; set; }
        public string? ResumeLink { get; set; }
        public string? CoverLetter { get; set; }

        public string ApplicationStatus { get; set; } = "Pending";

    }
}
