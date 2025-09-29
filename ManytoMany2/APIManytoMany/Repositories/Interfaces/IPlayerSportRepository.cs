using APIManytoMany.Models;

namespace APIManytoMany.Repositories.Interfaces
{
    public interface IPlayerSportRepository
    {
        Task<IEnumerable<PlayerSport>> GetAllAsync();
        Task<PlayerSport> AddAsync(PlayerSport playerSport);
    }
}
