using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var result = await _dashboardService.GetAdminDashboardAsync(from, to);
            return Ok(result);
        }

        [HttpGet("agent/{agentId}")]
        public async Task<IActionResult> GetAgentDashboard(int agentId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var result = await _dashboardService.GetAgentDashboardAsync(agentId, from, to);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDashboard(int userId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var result = await _dashboardService.GetUserDashboardAsync(userId, from, to);
            return Ok(result);
        }
    }
}
