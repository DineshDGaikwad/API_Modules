namespace APIJobPortal.DTOs.Auth
{
     public class RegisterDTO
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Mobile number for companies
        public string Role { get; set; } = string.Empty; // "JobSeeker" or "Company"
     public string? PhoneNumber { get; set; }
}

}
