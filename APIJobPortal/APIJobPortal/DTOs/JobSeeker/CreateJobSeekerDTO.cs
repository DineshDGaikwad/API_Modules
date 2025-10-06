namespace APIJobPortal.DTOs.JobSeeker
{
    public class CreateJobSeekerDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Skills { get; set; }
        public int ExperienceYears { get; set; }
        public string? ResumeLink { get; set; }
    }
}
