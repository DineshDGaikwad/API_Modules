using APIwithoutJunctionModel.Data;
using APIwithoutJunctionModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIwithoutJunctionModel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly DocPatDbContext _context;
        private readonly IConfiguration _config;

        public TokenController(DocPatDbContext context, IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [HttpPost]
        public IActionResult GenerateToken([FromBody] User loginUser)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.email == loginUser.email &&
                                     u.password == loginUser.password &&
                                     u.role == loginUser.role);

            if (user == null)
                return BadRequest(new { message = "Invalid email, password, or role" });

            // Retrieve key from appsettings.json
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                return StatusCode(500, "JWT Key is not configured.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.userName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.email ?? ""),
                new Claim(ClaimTypes.Role, user.role ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = jwt, role = user.role });
        }
    }
}
