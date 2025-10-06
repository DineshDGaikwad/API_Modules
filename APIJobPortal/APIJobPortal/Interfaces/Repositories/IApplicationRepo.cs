using APIJobPortal.Models;

namespace APIJobPortal.Interfaces.Repositories
{
    public interface IApplicationRepo
    {
        Task<JobApplication?> GetByIdAsync(int id);
        Task<IEnumerable<JobApplication>> GetAllAsync();
        Task<JobApplication> AddAsync(JobApplication application);
        Task<bool> DeleteAsync(int id);
    }
}
