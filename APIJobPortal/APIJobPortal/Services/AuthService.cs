using APIJobPortal.DTOs.Auth;
using APIJobPortal.Helpers;
using APIJobPortal.Interfaces.Services;
using APIJobPortal.Models;
using APIJobPortal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace APIJobPortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly JobPortalDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(JobPortalDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Register new JobSeeker or Company
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
                    // Validate role explicitly to avoid accidental registrations
            if (string.IsNullOrWhiteSpace(dto.Role))
                throw new Exception("Role is required. Use 'JobSeeker' or 'Company'.");

            var normalizedRole = dto.Role.Trim();
            if (!normalizedRole.Equals("JobSeeker", StringComparison.OrdinalIgnoreCase) &&
                !normalizedRole.Equals("Company", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid role. Allowed values: 'JobSeeker', 'Company'.");
            }
            // Check if email already exists
            if (await _context.JobSeekers.AnyAsync(js => js.Email == dto.Email) ||
                await _context.Companies.AnyAsync(c => c.Email == dto.Email))
            {
                throw new Exception("Email already registered.");
            }

            // Hash password
            string hashed = PasswordHasher.HashPassword(dto.Password);

            // Get JWT settings from configuration
            string secret = _config["Jwt:Secret"] 
                ?? throw new Exception("JWT Secret is not configured in appsettings.json");
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];
            int expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

            if (normalizedRole.Equals("Company", StringComparison.OrdinalIgnoreCase))
            {
                var company = new Company
                {
                    Name = dto.FullName,
                    Email = dto.Email,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                string token = JwtHelper.GenerateToken(company.CompanyId, company.Email!, "Company", secret, expiryMinutes, issuer, audience);
                return new AuthResponseDTO 
                { 
                    Token = token, 
                    Role = "Company", 
                    UserId = company.CompanyId, 
                    Email = company.Email! 
                };
            }
            else // JobSeeker
            {
                var seeker = new JobSeeker
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = hashed,
                    RegisteredAt = DateTime.UtcNow
                };

                _context.JobSeekers.Add(seeker);
                await _context.SaveChangesAsync();

                string token = JwtHelper.GenerateToken(seeker.JobSeekerId, seeker.Email!, "JobSeeker", secret, expiryMinutes, issuer, audience);
                return new AuthResponseDTO 
                { 
                    Token = token, 
                    Role = "JobSeeker", 
                    UserId = seeker.JobSeekerId, 
                    Email = seeker.Email! 
                };
            }
        }

        // Login existing user (JobSeeker or Company)
        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            string secret = _config["Jwt:Secret"] 
                ?? throw new Exception("JWT Secret is not configured in appsettings.json");
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];
            int expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

            // Check JobSeeker login
            var seeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.Email == dto.Email);
            if (seeker != null && PasswordHasher.VerifyPassword(seeker.PasswordHash, dto.Password))
            {
                string token = JwtHelper.GenerateToken(seeker.JobSeekerId, seeker.Email!, "JobSeeker", secret);
                return new AuthResponseDTO 
                { 
                    Token = token, 
                    Role = "JobSeeker", 
                    UserId = seeker.JobSeekerId, 
                    Email = seeker.Email! 
                };
            }

            // Check Company login (password not implemented yet)
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Email == dto.Email);
            if (company != null)
            {
                // If company has a password stored in future, verify here. For now generate token if email exists.
                string token = JwtHelper.GenerateToken(company.CompanyId, company.Email!, "Company", secret, expiryMinutes, issuer, audience);
                return new AuthResponseDTO 
                { 
                    Token = token, 
                    Role = "Company", 
                    UserId = company.CompanyId, 
                    Email = company.Email! 
                };
            }

            return null;
        }
    }
}
