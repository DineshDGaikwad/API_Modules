using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyOwnershipController : ControllerBase
    {
        private readonly IPropertyOwnershipService _service;

        public PropertyOwnershipController(IPropertyOwnershipService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _service.GetByIdAsync(id);
            return record == null ? NotFound("Ownership record not found.") : Ok(record);
        }

        [HttpGet("user/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int userId) =>
            Ok(await _service.GetByUserIdAsync(userId));

        [HttpGet("property/{propertyId:int}")]
        [Authorize]
        public async Task<IActionResult> GetByProperty(int propertyId) =>
            Ok(await _service.GetByPropertyIdAsync(propertyId));

        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Create(PropertyOwnershipCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return created
                ? Ok(new { message = "Ownership record created successfully." })
                : BadRequest("Failed to create ownership record.");
        }

        [HttpPut("verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Verify(PropertyOwnershipVerifyDto dto)
        {
            var updated = await _service.VerifyOwnershipAsync(dto);
            return updated
                ? Ok(new { message = dto.Verified ? "Ownership verified successfully." : "Ownership rejected." })
                : NotFound("Ownership record not found.");
        }

        [HttpPut("transfer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Transfer(PropertyOwnershipTransferDto dto)
        {
            var transferred = await _service.TransferOwnershipAsync(dto);
            return transferred
                ? Ok(new { message = "Ownership transferred successfully." })
                : BadRequest("Ownership transfer failed.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted
                ? Ok(new { message = "Ownership deleted successfully." })
                : NotFound("Ownership record not found.");
        }
    }
}
