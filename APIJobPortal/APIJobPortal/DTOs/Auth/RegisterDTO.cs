namespace APIJobPortal.DTOs.Auth
{
    public class RegisterDTO
    {
        public string FullName { get; set; } = string.Empty; // JobSeeker or Company name
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "JobSeeker"; // or "Company"
    }
}
