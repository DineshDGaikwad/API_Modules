using APIAssessment.Models;

namespace APIAssessment.Interfaces
{
    public interface IShowService
    {
        Task<IEnumerable<Show>> GetShowsByDateAsync(DateTime date);
        Task<bool> AddShowAsync(Show show);
    }
}
