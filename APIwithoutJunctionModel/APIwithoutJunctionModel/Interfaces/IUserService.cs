using APIwithoutJunctionModel.DTOs;
using APIwithoutJunctionModel.Models;

namespace APIwithoutJunctionModel.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(CreateUserDTO dto);
        Task<bool> UpdateAsync(int id, CreateUserDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
