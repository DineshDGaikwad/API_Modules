using APIJobPortal.Data;
using APIJobPortal.Interfaces.Repositories;
using APIJobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIJobPortal.Repository
{
    public class JobRepository : IJobRepo
    {
        private readonly JobPortalDbContext _context;

        public JobRepository(JobPortalDbContext context)
        {
            _context = context;
        }

        public async Task<Job> AddAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _context.Jobs.ToListAsync();
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs.FindAsync(id);
        }
    }
}
