using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyTransactionController : ControllerBase
    {
        private readonly IPropertyTransactionService _service;

        public PropertyTransactionController(IPropertyTransactionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PropertyTransactionCreateDto dto)
        {
            try
            {
                var result = await _service.CreateTransactionAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.TransactionId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result is null ? NotFound("Transaction not found.") : Ok(result);
        }

        [HttpGet("buyer/{buyerId:int}")]
        public async Task<IActionResult> GetByBuyer(int buyerId) 
            => Ok(await _service.GetByBuyerAsync(buyerId));

        [HttpGet("seller/{sellerId:int}")]
        public async Task<IActionResult> GetBySeller(int sellerId) 
            => Ok(await _service.GetBySellerAsync(sellerId));

        [HttpGet("agent/{agentId:int}")]
        public async Task<IActionResult> GetByAgent(int agentId, [FromQuery] string? status, [FromQuery] bool includeArchived = false)
        {
            var result = await _service.GetByAgentAsync(agentId, status, includeArchived);
            return Ok(result);
        }

        [HttpGet("pending/admin")]
        public async Task<IActionResult> GetPendingForAdmin()
            => Ok(await _service.GetPendingForAdminAsync());

        [HttpPut("agent/decision")]
        [HttpPost("agent/decision")]
        public async Task<IActionResult> AgentDecision(AgentTransactionDecisionDto dto)
        {
            var success = await _service.SubmitAgentDecisionAsync(dto);
            if (!success) return BadRequest("Unable to process agent decision.");

            return Ok(new
            {
                message = dto.Approve
                    ? "Request escalated to admin for final approval."
                    : "Request rejected by agent."
            });
        }

        [HttpPut("admin/decision")]
        [HttpPost("admin/decision")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDecision(AdminTransactionDecisionDto dto)
        {
            try
            {
                if (dto.AdminId <= 0)
                {
                    var claimValue = User.FindFirst("userId")?.Value;
                    if (int.TryParse(claimValue, out var claimAdminId) && claimAdminId > 0)
                        dto.AdminId = claimAdminId;
                }

                if (dto.AdminId <= 0)
                    return BadRequest("Admin identifier is required.");

                var success = await _service.SubmitAdminDecisionAsync(dto);
                if (!success) return BadRequest("Unable to process admin decision.");

                return Ok(new
                {
                    message = dto.Approve
                        ? "Transaction approved and ownership transferred."
                        : "Transaction rejected by admin."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("verify")]
        public async Task<IActionResult> Verify(PropertyTransactionVerifyDto dto)
        {
            var success = await _service.VerifyTransactionAsync(dto);
            if (!success) return NotFound("Transaction not found or update failed.");

            return Ok(new
            {
                message = dto.Approve
                    ? "Transaction approved and ownership transferred."
                    : "Transaction rejected."
            });
        }
    }
}
