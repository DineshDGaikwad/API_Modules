using APIJobPortal.DTOs.Job;
using APIJobPortal.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIJobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }
        
        [Authorize(Roles = "JobSeeker")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null) return NotFound();
            return Ok(job);
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDTO dto)
        {
            var job = await _jobService.CreateJobAsync(dto);
            return CreatedAtAction(nameof(GetJobById), new { id = job.JobId }, job);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            bool deleted = await _jobService.DeleteJobAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
