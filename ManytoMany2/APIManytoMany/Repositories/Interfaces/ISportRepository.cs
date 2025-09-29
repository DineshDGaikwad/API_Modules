using APIManytoMany.Models;

namespace APIManytoMany.Repositories.Interfaces
{
    public interface ISportRepository
    {
        Task<IEnumerable<Sport>> GetAllAsync();
        Task<Sport?> GetByIdAsync(int id);
        Task<Sport> AddAsync(Sport sport);
        Task UpdateAsync(Sport sport);
        Task DeleteAsync(int id);
    }
}
