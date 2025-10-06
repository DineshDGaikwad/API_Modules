using APIwithoutJunctionModel.Data;
using APIwithoutJunctionModel.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly SymmetricSecurityKey _key;

        public TokenController(DocPatDbContext context, IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Key"]!));
        }


        [HttpPost]
        public IActionResult GenerateToken([FromBody] User loginUser)
        {
            var user = _context.Users
                        .FirstOrDefault(u => u.email == loginUser.email
                                          && u.password == loginUser.password
                                          && u.role == loginUser.role);

            if (user == null)
                return BadRequest(new { message = "Invalid email, password, or role" });

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.userName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.email ?? ""),
                new Claim(ClaimTypes.Role, user.role ?? "")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = creds,
                Issuer = "YourAPI",
                Audience = "YourAPIUsers"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt, role = user.role });
        }
    }
}
