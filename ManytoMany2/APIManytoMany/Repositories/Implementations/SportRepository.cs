using Microsoft.EntityFrameworkCore;
using APIManytoMany.Data;
using APIManytoMany.Models;
using APIManytoMany.Repositories.Interfaces;

namespace APIManytoMany.Repositories.Implementations
{
    public class SportRepository : ISportRepository
    {
        private readonly AppDbContext _context;

        public SportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sport>> GetAllAsync()
        {
            return await _context.Sports
                .Include(s => s.PlayerSports)
                .ThenInclude(ps => ps.Player)
                .ToListAsync();
        }

        public async Task<Sport?> GetByIdAsync(int id)
        {
            return await _context.Sports
                .Include(s => s.PlayerSports)
                .ThenInclude(ps => ps.Player)
                .FirstOrDefaultAsync(s => s.SportId == id);
        }

        public async Task<Sport> AddAsync(Sport sport)
        {
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();
            return sport;
        }

        public async Task UpdateAsync(Sport sport)
        {
            _context.Entry(sport).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sport = await _context.Sports.FindAsync(id);
            if (sport != null)
            {
                _context.Sports.Remove(sport);
                await _context.SaveChangesAsync();
            }
        }
    }
}
