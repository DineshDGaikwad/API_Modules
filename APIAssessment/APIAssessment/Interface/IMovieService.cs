using APIAssessment.Models;

namespace APIAssessment.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<bool> AddAsync(Movie movie);
    }
}
