using APIPropertyRegistry.DTOs.PropertyDtos;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _service;

        public PropertyController(IPropertyService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var property = await _service.GetByIdAsync(id);
            return property is null
                ? NotFound(new { message = "Property not found." })
                : Ok(property);
        }

        [HttpGet("owner/{ownerId:int}")]
        public async Task<IActionResult> GetByOwnerId(int ownerId)
        {
            var result = await _service.GetByOwnerIdAsync(ownerId);
            return Ok(result);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetPropertiesByUser(int userId)
        {
            var result = await _service.GetByOwnerIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var result = await _service.GetAvailablePropertiesAsync();
            return Ok(result);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetPendingAsync();
            return Ok(result);
        }

        [HttpGet("approved")]
        public async Task<IActionResult> GetApproved()
        {
            var result = await _service.GetApprovedAsync();
            return Ok(result);
        }

        [HttpGet("for-sale")]
        public async Task<IActionResult> GetForSale()
        {
            var result = await _service.GetForSaleAsync();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? query,
            [FromQuery] string? status,
            [FromQuery] string? city,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var result = await _service.SearchAsync(query, status, city, minPrice, maxPrice);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PropertyCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var files = dto.Documents?
                .Where(file => file != null && file.Length > 0)
                .ToList() ?? new List<IFormFile>();

            if (!files.Any())
                return BadRequest(new { message = "At least one PDF document is required for property submission." });

            var allowedExtensions = new[] { ".pdf" };
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new { message = $"File '{file.FileName}' is not a valid PDF." });

                if (file.Length > 10 * 1024 * 1024)
                    return BadRequest(new { message = $"File '{file.FileName}' exceeds 10 MB size limit." });
            }

            dto.Documents = files;

            try
            {
                var created = await _service.CreateAsync(dto);
                if (created == null)
                    return BadRequest(new { message = "Failed to create property." });

                return CreatedAtAction(nameof(GetById), new { id = created.PropertyId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred while creating the property.",
                    details = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PropertyUpdateDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated
                ? Ok(new { message = "Property updated successfully." })
                : NotFound(new { message = "Property not found." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted
                ? Ok(new { message = "Property deleted successfully." })
                : NotFound(new { message = "Property not found." });
        }

        [HttpPut("approve")]
        public async Task<IActionResult> Approve([FromBody] PropertyApprovalDto dto)
        {
            var approved = await _service.ApprovePropertyAsync(dto.PropertyId, dto.Approve, dto.AdminId, dto.Remarks);
            return approved
                ? Ok(new { message = dto.Approve ? "Property approved successfully." : "Property rejected successfully." })
                : NotFound(new { message = "Property not found." });
        }


        [HttpPut("{propertyId:int}/sell")]
        public async Task<IActionResult> MarkPropertyForSaleById(int propertyId, [FromBody] PropertySellDto dto)
        {
            dto.PropertyId = propertyId;
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                dto.OwnerId = userId;

            try
            {
                var result = await _service.MarkPropertyForSaleAsync(dto);
                return result
                    ? Ok(new { message = "Property listed successfully." })
                    : BadRequest(new { message = "Failed to list property for sale. Please verify property and agent details." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred while marking the property for sale.",
                    details = ex.Message
                });
            }
        }

[HttpPut("{propertyId:int}/remove-sale")]
public async Task<IActionResult> RemoveFromSale(int propertyId)
{
    try
    {
        var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid or missing user token." });

        var result = await _service.RemovePropertyFromSaleAsync(propertyId, userId);
        return result
            ? Ok(new { message = "Property removed from sale successfully." })
            : BadRequest(new { message = "Failed to remove property from sale. Please verify ownership." });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new
        {
            message = "An unexpected error occurred while removing the property from sale.",
            details = ex.Message
        });
    }
}


    }
}
