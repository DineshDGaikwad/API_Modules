using APIJobPortal.Data;
using APIJobPortal.Interfaces.Repositories;
using APIJobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace APIJobPortal.Repository
{
    public class CompanyRepository : ICompanyRepo
    {
        private readonly JobPortalDbContext _context;

        public CompanyRepository(JobPortalDbContext context)
        {
            _context = context;
        }

        public async Task<Company> AddAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null) return false;

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task<Company?> GetByEmailAsync(string email)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
