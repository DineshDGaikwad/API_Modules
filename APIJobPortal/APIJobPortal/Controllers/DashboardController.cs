using APIJobPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIJobPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly JobPortalDbContext _context;

        public DashboardController(JobPortalDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalJobSeekers = await _context.JobSeekers.CountAsync();
            var totalCompanies = await _context.Companies.CountAsync();
            var totalJobs = await _context.Jobs.CountAsync();
            var totalApplications = await _context.JobApplications.CountAsync();

            return Ok(new
            {
                TotalJobSeekers = totalJobSeekers,
                TotalCompanies = totalCompanies,
                TotalJobs = totalJobs,
                TotalApplications = totalApplications
            });
        }

        [HttpGet("company-applications")]
        public async Task<IActionResult> GetApplicationsPerCompany()
        {
            var data = await _context.Companies
                .Select(c => new
                {
                    CompanyName = c.Name,
                    TotalJobs = c.CompanyJobs.Count,
                    TotalApplications = c.CompanyJobs.SelectMany(j => j.Job.JobApplications).Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("job-applications")]
        public async Task<IActionResult> GetApplicationsPerJob()
        {
            var data = await _context.Jobs
                .Select(j => new
                {
                    JobTitle = j.Title,
                    TotalApplications = j.JobApplications.Count
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
