using APIAssessment.Models;
using APIAssessment.Interfaces;
using APIAssessment.Repositories;

namespace APIAssessment.Services
{
    public class MovieService : IMovieService
    {
        private readonly IRepository<Movie> _repo;
        public MovieService(IRepository<Movie> repo) { _repo = repo; }

        public async Task<IEnumerable<Movie>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<bool> AddAsync(Movie movie)
        {
            await _repo.AddAsync(movie);
            return await _repo.SaveAsync();
        }
    }
}
