using APIAssessment.Data;
using APIAssessment.Interfaces;
using APIAssessment.Models;
using Microsoft.EntityFrameworkCore;

namespace APIAssessment.Services
{
    public class ShowService : IShowService
    {
        private readonly AppDbContext _context;
        public ShowService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Show>> GetShowsByDateAsync(DateTime date) =>
            await _context.Shows
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .Where(s => s.StartsAt.Date == date.Date)
                .ToListAsync();

        public async Task<bool> AddShowAsync(Show show)
        {
            var overlapping = await _context.Shows
                .Where(s => s.ScreenId == show.ScreenId &&
                            s.StartsAt < show.EndsAt &&
                            show.StartsAt < s.EndsAt)
                .AnyAsync();
            if (overlapping) return false;

            _context.Shows.Add(show);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
