using APIJobPortal.Data;
using APIJobPortal.Interfaces.Repositories;
using APIJobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIJobPortal.Repositories
{
    public class JobSeekerRepository : IJobSeekerRepo
    {
        private readonly JobPortalDbContext _context;

        public JobSeekerRepository(JobPortalDbContext context)
        {
            _context = context;
        }

        public async Task<JobSeeker> AddAsync(JobSeeker jobSeeker)
        {
            _context.JobSeekers.Add(jobSeeker);
            await _context.SaveChangesAsync();
            return jobSeeker;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seeker = await _context.JobSeekers.FindAsync(id);
            if (seeker == null) return false;

            _context.JobSeekers.Remove(seeker);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<JobSeeker>> GetAllAsync()
        {
            return await _context.JobSeekers.ToListAsync();
        }

        public async Task<JobSeeker?> GetByIdAsync(int id)
        {
            return await _context.JobSeekers.FindAsync(id);
        }

        public async Task<JobSeeker?> GetByEmailAsync(string email)
        {
            return await _context.JobSeekers.FirstOrDefaultAsync(js => js.Email == email);
        }
    }
}
