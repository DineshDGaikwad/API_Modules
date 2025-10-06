using APIJobPortal.Models;

namespace APIJobPortal.Interfaces.Repositories
{
    public interface IJobSeekerRepo
    {
        Task<JobSeeker?> GetByIdAsync(int id);
        Task<IEnumerable<JobSeeker>> GetAllAsync();
        Task<JobSeeker> AddAsync(JobSeeker jobSeeker);
        Task<bool> DeleteAsync(int id);
        Task<JobSeeker?> GetByEmailAsync(string email);
    }
}
