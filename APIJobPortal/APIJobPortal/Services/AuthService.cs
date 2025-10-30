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

        // ✅ Register JobSeeker or Company (Company uses ContactNumber as "password")
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Role))
                throw new Exception("Role is required. Use 'JobSeeker' or 'Company'.");

            string role = dto.Role.Trim();

            if (!role.Equals("JobSeeker", StringComparison.OrdinalIgnoreCase) &&
                !role.Equals("Company", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Invalid role. Allowed values: 'JobSeeker' or 'Company'.");

            // ✅ Check duplicate email
            bool exists = await _context.JobSeekers.AnyAsync(x => x.Email == dto.Email)
                       || await _context.Companies.AnyAsync(x => x.Email == dto.Email);
            if (exists)
                throw new Exception("Email already registered.");

            // ✅ JWT configuration
            string secret = _config["Jwt:Secret"] ?? throw new Exception("JWT Secret missing.");
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];
            int expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

            if (role.Equals("Company", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(dto.Password))
                    throw new Exception("Mobile number is required for Company registration.");

                var company = new Company
                {
                    Name = dto.FullName,
                    Email = dto.Email,
                    ContactNumber = dto.PhoneNumber ?? dto.Password,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                string token = JwtHelper.GenerateToken(
                    company.CompanyId, company.Email!, "Company",
                    secret, expiryMinutes, issuer, audience);

                return new AuthResponseDTO
                {
                    Token = token,
                    Role = "Company",
                    UserId = company.CompanyId,
                    Email = company.Email!
                };
            }
            else
            {
                // ✅ JobSeeker still uses password hashing
                string hashed = PasswordHasher.HashPassword(dto.Password);

                var seeker = new JobSeeker
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = hashed,
                    RegisteredAt = DateTime.UtcNow
                };

                _context.JobSeekers.Add(seeker);
                await _context.SaveChangesAsync();

                string token = JwtHelper.GenerateToken(
                    seeker.JobSeekerId, seeker.Email!, "JobSeeker",
                    secret, expiryMinutes, issuer, audience);

                return new AuthResponseDTO
                {
                    Token = token,
                    Role = "JobSeeker",
                    UserId = seeker.JobSeekerId,
                    Email = seeker.Email!
                };
            }
        }

        // ✅ Login: JobSeeker (email + password), Company (email + mobile number)
        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            string secret = _config["Jwt:Secret"] ?? throw new Exception("JWT Secret missing.");
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];
            int expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

            // ✅ Check JobSeeker
            var seeker = await _context.JobSeekers.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (seeker != null && PasswordHasher.VerifyPassword(seeker.PasswordHash, dto.Password))
            {
                string token = JwtHelper.GenerateToken(
                    seeker.JobSeekerId, seeker.Email!, "JobSeeker",
                    secret, expiryMinutes, issuer, audience);

                return new AuthResponseDTO
                {
                    Token = token,
                    Role = "JobSeeker",
                    UserId = seeker.JobSeekerId,
                    Email = seeker.Email!
                };
            }

            // ✅ Check Company using ContactNumber as password
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (company != null && company.ContactNumber == dto.Password)
            {
                string token = JwtHelper.GenerateToken(
                    company.CompanyId, company.Email!, "Company",
                    secret, expiryMinutes, issuer, audience);

                return new AuthResponseDTO
                {
                    Token = token,
                    Role = "Company",
                    UserId = company.CompanyId,
                    Email = company.Email!
                };
            }

            return null; // ❌ Invalid credentials
        }
    }
}
