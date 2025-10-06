using APIJobPortal.Models;

namespace APIJobPortal.Interfaces.Repositories
{
    public interface IJobRepo
    {
        Task<Job?> GetByIdAsync(int id);
        Task<IEnumerable<Job>> GetAllAsync();
        Task<Job> AddAsync(Job job);
        Task<bool> DeleteAsync(int id);
    }
}
