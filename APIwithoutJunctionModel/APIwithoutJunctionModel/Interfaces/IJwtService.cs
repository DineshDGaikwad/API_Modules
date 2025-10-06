using APIwithoutJunctionModel.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIwithoutJunctionModel.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateToken(User user)
        {
            var keyString = _config["Jwt:Key"] 
                            ?? throw new ArgumentNullException("JWT Key is missing in appsettings.json");
            var key = Encoding.ASCII.GetBytes(keyString);

            var durationString = _config["Jwt:DurationInMinutes"];
            if (!double.TryParse(durationString, out var duration))
            {
                throw new Exception("JWT DurationInMinutes is missing or invalid in appsettings.json");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                new Claim(ClaimTypes.Name, user.userName ?? ""),
                new Claim(ClaimTypes.Role, user.role ?? "")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(duration),
                Issuer = _config["Jwt:Issuer"] ?? throw new Exception("JWT Issuer missing"),
                Audience = _config["Jwt:Audience"] ?? throw new Exception("JWT Audience missing"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
