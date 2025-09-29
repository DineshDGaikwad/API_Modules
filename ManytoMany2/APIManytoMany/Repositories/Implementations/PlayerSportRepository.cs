using Microsoft.EntityFrameworkCore;
using APIManytoMany.Data;
using APIManytoMany.Models;
using APIManytoMany.Repositories.Interfaces;

namespace APIManytoMany.Repositories.Implementations
{
    public class PlayerSportRepository : IPlayerSportRepository
    {
        private readonly AppDbContext _context;

        public PlayerSportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlayerSport>> GetAllAsync()
        {
            return await _context.PlayerSports
                .Include(ps => ps.Player)
                .Include(ps => ps.Sport)
                .ToListAsync();
        }

        public async Task<PlayerSport> AddAsync(PlayerSport playerSport)
        {
            _context.PlayerSports.Add(playerSport);
            await _context.SaveChangesAsync();
            return playerSport;
        }
    }
}
