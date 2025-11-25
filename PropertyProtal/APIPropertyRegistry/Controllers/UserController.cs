using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Services.Interfaces;
using APIPropertyRegistry.Services.Implementations;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public UserController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.RegisterUserAsync(dto);
            if (!success)
                return BadRequest("Registration failed. Email may already exist.");

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.LoginAsync(dto);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",
                token,
                user = new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Role,
                    user.IsApproved
                }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("userId")?.Value;
            var role = User.FindFirst("role")?.Value;
            var email = User.Identity?.Name;

            return Ok(new
            {
                message = "Token validated successfully",
                userId,
                role,
                email
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound("User not found.") : Ok(user);
        }

        [Authorize]
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return user == null ? NotFound("User not found.") : Ok(user);
        }

        [Authorize]
        [HttpGet("by-role/{role}")]
        public async Task<IActionResult> GetByRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return BadRequest("Role is required.");

            try
            {
                var normalizedRole = role.Trim();
                var users = await _userService.GetUsersByRoleAsync(normalizedRole);

                if (users == null || !users.Any())
                    return NotFound($"No users found with role '{normalizedRole}'.");

                var currentRole = User.FindFirst("role")?.Value ?? string.Empty;
                var isAdmin = currentRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);

                if (!isAdmin && normalizedRole.Equals("Agent", StringComparison.OrdinalIgnoreCase))
                    users = users.Where(u => u.IsApproved).ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] string? role = null)
        {
            var users = await _userService.SearchUsersAsync(query ?? string.Empty, role);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("~/api/Agent/search")]
        public async Task<IActionResult> SearchAgents([FromQuery] string? query)
        {
            var users = await _userService.SearchUsersAsync(query ?? string.Empty, "Agent");
            return Ok(users);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.UpdateUserAsync(id, dto);
            return success
                ? Ok(new { message = "User updated successfully." })
                : NotFound("User not found or update failed.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            return success
                ? Ok(new { message = "User deleted successfully." })
                : NotFound("User not found.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending-agents")]
        public async Task<IActionResult> GetPendingAgents()
        {
            var agents = await _userService.GetPendingAgentsAsync();
            return Ok(agents);
        }

        [Authorize]
        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromForm] UserProfileUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _userService.UpdateProfileAsync(
                    id,
                    dto.ProfileImage,
                    dto.FullName,
                    dto.MobileNumber
                );

                if (updated == null)
                    return BadRequest(new { message = "Profile update failed." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating profile.",
                    error = ex.Message
                });
            }
        }
    }
}
