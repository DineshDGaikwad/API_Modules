using APIJobPortal.DTOs.JobSeeker;
using APIJobPortal.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIJobPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobSeekersController : ControllerBase
    {
        private readonly IJobSeekerService _service;

        public JobSeekersController(IJobSeekerService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var seeker = await _service.GetJobSeekerByIdAsync(id);
            if (seeker == null) return NotFound();
            return Ok(seeker);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var seekers = await _service.GetAllJobSeekersAsync();
            return Ok(seekers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJobSeekerDTO dto)
        {
            var seeker = await _service.CreateJobSeekerAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = seeker.JobSeekerId }, seeker);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteJobSeekerAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
