using APIJobPortal.DTOs.Application;
using APIJobPortal.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIJobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            var apps = await _applicationService.GetAllApplicationsAsync();
            return Ok(apps);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationById(int id)
        {
            var app = await _applicationService.GetApplicationByIdAsync(id);
            if (app == null) return NotFound();
            return Ok(app);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDTO dto)
        {
            var app = await _applicationService.CreateApplicationAsync(dto);
            return CreatedAtAction(nameof(GetApplicationById), new { id = app.ApplicationId }, app);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            bool deleted = await _applicationService.DeleteApplicationAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
