using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("agents/pending")]
        public async Task<IActionResult> GetPendingAgents()
        {
            var agents = await _adminService.GetPendingAgentsAsync();
            return Ok(agents);
        }

        [HttpGet("agents/approved")]
        public async Task<IActionResult> GetApprovedAgents()
        {
            var agents = await _adminService.GetApprovedAgentsAsync();
            return Ok(agents);
        }

        [HttpPut("agents/{agentId}/approval")]
        public async Task<IActionResult> ApproveOrRejectAgent(int agentId, [FromBody] ApproveAgentDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid approval request.");

            var success = await _adminService.ApproveOrRejectAgentAsync(agentId, dto.Approve, dto.Remarks);

            if (!success)
                return NotFound("Agent not found.");

            var message = dto.Approve
                ? "Agent approved successfully."
                : "Agent rejected successfully.";

            return Ok(new { success = true, message, remarks = dto.Remarks });
        }

        [HttpGet("properties/pending")]
        public async Task<IActionResult> GetPendingProperties()
        {
            var props = await _adminService.GetPendingPropertiesAsync();
            return Ok(props);
        }

        [HttpGet("properties/approved")]
        public async Task<IActionResult> GetApprovedProperties()
        {
            var props = await _adminService.GetApprovedPropertiesAsync();
            return Ok(props);
        }

        [HttpPut("properties/{propertyId}/approval/{adminId}")]
        public async Task<IActionResult> ApproveOrRejectProperty(int propertyId, int adminId, [FromBody] ApprovePropertyDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid approval request.");

            var success = await _adminService.ApproveOrRejectPropertyAsync(propertyId, adminId, dto.Approve, dto.Remarks);

            if (!success)
                return NotFound("Property not found.");

            var message = dto.Approve
                ? "Property approved successfully."
                : "Property rejected successfully.";

            return Ok(new { success = true, message, remarks = dto.Remarks });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("agents")]
        public async Task<IActionResult> GetAllAgents()
        {
            var agents = await _adminService.GetAllAgentsAsync();
            return Ok(agents);
        }

        [HttpGet("properties")]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _adminService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetRecentTransactions([FromQuery] int limit = 20)
        {
            var transactions = await _adminService.GetRecentTransactionsAsync(limit);
            return Ok(transactions);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string type = "all")
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter is required.");

            var results = await _adminService.SearchAsync(query, type);
            return Ok(results);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid user data.");

            var success = await _adminService.UpdateUserAsync(id, dto);
            if (!success)
                return NotFound("User not found.");

            return Ok(new { success = true, message = "User updated successfully." });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _adminService.DeleteUserAsync(id);
            if (!success)
                return NotFound("User not found.");

            return Ok(new { success = true, message = "User deleted successfully." });
        }

        [HttpDelete("properties/{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var success = await _adminService.DeletePropertyAsync(id);
            if (!success)
                return NotFound("Property not found.");

            return Ok(new { success = true, message = "Property deleted successfully." });
        }
    }
}
