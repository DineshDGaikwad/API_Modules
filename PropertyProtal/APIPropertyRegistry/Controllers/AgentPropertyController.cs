using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentPropertyController : ControllerBase
    {
        private readonly IAgentPropertyService _service;

        public AgentPropertyController(IAgentPropertyService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound("Assignment not found.") : Ok(result);
        }

        [HttpGet("agent/{agentId:int}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> GetByAgent(int agentId) =>
            Ok(await _service.GetByAgentAsync(agentId));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AgentPropertyCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.AgentPropertyId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(AgentPropertyApproveDto dto)
        {
            var success = await _service.ApproveAsync(dto);
            return success
                ? Ok(new { message = dto.Approve ? "Agent assigned successfully." : "Assignment revoked." })
                : NotFound("Assignment not found.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted
                ? Ok(new { message = "Assignment deleted successfully." })
                : NotFound("Assignment not found.");
        }
    }
}
