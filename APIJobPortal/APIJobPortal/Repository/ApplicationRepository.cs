using APIJobPortal.Data;
using APIJobPortal.Interfaces.Repositories;
using APIJobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIJobPortal.Repository
{
    public class ApplicationRepository : IApplicationRepo
    {
        private readonly JobPortalDbContext _context;

        public ApplicationRepository(JobPortalDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication> AddAsync(JobApplication application)
        {
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var app = await _context.JobApplications.FindAsync(id);
            if (app == null) return false;

            _context.JobApplications.Remove(app);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<JobApplication>> GetAllAsync()
        {
            return await _context.JobApplications
                .Include(a => a.JobSeeker)
                .Include(a => a.Job)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications
                .Include(a => a.JobSeeker)
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);
        }
    }
}
