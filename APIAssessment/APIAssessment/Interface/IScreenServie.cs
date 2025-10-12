using APIAssessment.Models;

namespace APIAssessment.Interfaces
{
    public interface IScreenService
    {
        Task<IEnumerable<Screen>> GetAllAsync();
        Task<bool> AddAsync(Screen screen);
    }
}
