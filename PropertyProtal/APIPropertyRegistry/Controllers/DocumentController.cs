using APIPropertyRegistry.DTOs;
using APIPropertyRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;

        public DocumentController(IDocumentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending() =>
            Ok(await _service.GetPendingAsync());

        [HttpGet("property/{propertyId}")]
        [Authorize]
        public async Task<IActionResult> GetByProperty(int propertyId)
        {
            var result = await _service.GetByPropertyAsync(propertyId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound("Document not found.") : Ok(result);
        }

        [HttpGet("view")]
        [Authorize]
        public async Task<IActionResult> ViewDocument([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL is required.");

            try
            {
                using var http = new HttpClient();
                var pdfBytes = await http.GetByteArrayAsync(url);

                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unable to load file", error = ex.Message });
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] DocumentCreateDto dto, IFormFile file)
        {
            try
            {
                var created = await _service.CreateAsync(dto, file);
                return CreatedAtAction(nameof(GetById), new { id = created.DocumentId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Verify([FromBody] DocumentVerifyDto dto)
        {
            var success = await _service.VerifyAsync(dto);
            return success
                ? Ok(new { message = dto.Verified ? "Document verified successfully." : "Document marked unverified." })
                : NotFound("Document not found.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? Ok(new { message = "Document deleted." }) : NotFound("Document not found.");
        }
    }
}
