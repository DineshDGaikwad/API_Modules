using APIJobPortal.Models;

namespace APIJobPortal.Interfaces.Repositories
{
    public interface ICompanyRepo
    {
        Task<Company?> GetByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company> AddAsync(Company company);
        Task<bool> DeleteAsync(int id);
        Task<Company?> GetByEmailAsync(string email);
    }
}
