using APIwithoutJunctionModel.DTOs;
using APIwithoutJunctionModel.Models;

namespace APIwithoutJunctionModel.Interfaces
{
    public interface IUserRepo
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
