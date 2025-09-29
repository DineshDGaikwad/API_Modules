using Microsoft.EntityFrameworkCore;
using APIManytoMany.Data;
using APIManytoMany.Models;
using APIManytoMany.Repositories.Interfaces;

namespace APIManytoMany.Repositories.Implementations
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _context;

        public PlayerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players
                .Include(p => p.PlayerSports)
                .ThenInclude(ps => ps.Sport)
                .ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.PlayerSports)
                .ThenInclude(ps => ps.Sport)
                .FirstOrDefaultAsync(p => p.PlayerId == id);
        }

        public async Task<Player> AddAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task UpdateAsync(Player player)
        {
            _context.Entry(player).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
        }
    }
}
