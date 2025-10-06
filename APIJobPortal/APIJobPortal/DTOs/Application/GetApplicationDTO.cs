namespace APIJobPortal.DTOs.Application
{
    public class GetApplicationDTO
    {
        public int ApplicationId { get; set; }
        public int JobSeekerId { get; set; }
        public int JobId { get; set; }
        public string ApplicationStatus { get; set; } = "Pending";
        public string? ResumeLink { get; set; }
        public string? CoverLetter { get; set; }
        public DateTime AppliedOn { get; set; }
    }
}
