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

        // 1️⃣ Overall Summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalJobSeekers = await _context.JobSeekers.CountAsync();
            var totalCompanies = await _context.Companies.CountAsync();
            var totalJobs = await _context.Jobs.CountAsync();
            var totalApplications = await _context.JobApplications.CountAsync();

            var activeJobs = await _context.Jobs.CountAsync(j => j.ExpiryDate == null || j.ExpiryDate > DateTime.UtcNow);
            var recentApplications = await _context.JobApplications
                .Where(a => a.AppliedOn >= DateTime.UtcNow.AddDays(-30))
                .CountAsync();

            return Ok(new
            {
                TotalJobSeekers = totalJobSeekers,
                TotalCompanies = totalCompanies,
                TotalJobs = totalJobs,
                ActiveJobs = activeJobs,
                TotalApplications = totalApplications,
                RecentApplications = recentApplications
            });
        }

        // 2️⃣ Applications per Company
        [HttpGet("company-applications")]
        public async Task<IActionResult> GetApplicationsPerCompany()
        {
            var data = await _context.Companies
                .Include(c => c.CompanyJobs)
                    .ThenInclude(cj => cj.Job)
                        .ThenInclude(j => j.JobApplications)
                .Select(c => new
                {
                    CompanyName = c.Name,
                    TotalJobs = c.CompanyJobs.Count,
                    TotalApplications = c.CompanyJobs
                        .Sum(cj => cj.Job.JobApplications.Count)
                })
                .OrderByDescending(c => c.TotalApplications)
                .ToListAsync();

            return Ok(data);
        }

        // 3️⃣ Applications per Job
        [HttpGet("job-applications")]
        public async Task<IActionResult> GetApplicationsPerJob()
        {
            var data = await _context.Jobs
                .Include(j => j.JobApplications)
                .Select(j => new
                {
                    JobTitle = j.Title,
                    TotalApplications = j.JobApplications.Count
                })
                .OrderByDescending(j => j.TotalApplications)
                .Take(10)
                .ToListAsync();

            return Ok(data);
        }

        // 4️⃣ Top Skills Among Job Seekers
        [HttpGet("top-skills")]
        public async Task<IActionResult> GetTopSkills()
        {
            var data = await _context.JobSeekers
                .Where(js => !string.IsNullOrEmpty(js.Skills))
                .SelectMany(js => js.Skills.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(skill => skill)
                .Select(g => new
                {
                    Skill = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            return Ok(data);
        }

        // 5️⃣ Monthly Application Trends
        [HttpGet("application-trends")]
        public async Task<IActionResult> GetMonthlyApplicationTrends()
        {
            var trends = await _context.JobApplications
                .GroupBy(a => new { a.AppliedOn.Year, a.AppliedOn.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Count = g.Count()
                })
                .OrderBy(g => g.Month)
                .ToListAsync();

            return Ok(trends);
        }

        // 6️⃣ Recent Applications Overview
        [HttpGet("recent-applications")]
        public async Task<IActionResult> GetRecentApplications()
        {
            var data = await _context.JobApplications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                .OrderByDescending(a => a.AppliedOn)
                .Take(5)
                .Select(a => new
                {
                    a.ApplicationId,
                    JobTitle = a.Job.Title,
                    SeekerName = a.JobSeeker.FullName,
                    a.ApplicationStatus,
                    AppliedOn = a.AppliedOn.ToString("dd MMM yyyy")
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("recent-jobs")]
public async Task<IActionResult> GetRecentJobs()
{
    var data = await _context.Jobs
        .Include(j => j.CompanyJobs)
        .OrderByDescending(j => j.PostedDate)
        .Take(5)
        .Select(j => new
        {
            j.JobId,
            JobTitle = j.Title,
            CompanyName = j.CompanyJobs
                .Select(cj => cj.Company.Name)
                .FirstOrDefault(),
            Location = j.Location,
            Salary = j.Salary,
            PostedOn = j.PostedDate.ToString("dd MMM yyyy"),
            TotalApplications = j.JobApplications.Count
        })
        .ToListAsync();

    return Ok(data);
}

    }
}
