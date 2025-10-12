namespace APIJobPortal.DTOs.Auth
{
    public class RegisterDTO
    {
        public string FullName { get; set; } = string.Empty; // JobSeeker or Company name
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        // Require explicit role in request; default to empty to avoid accidental JobSeeker registration
        public string Role { get; set; } = string.Empty; // "JobSeeker" or "Company"
    }
}
