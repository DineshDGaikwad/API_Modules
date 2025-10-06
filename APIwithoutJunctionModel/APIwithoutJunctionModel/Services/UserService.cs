using APIwithoutJunctionModel.DTOs;
using APIwithoutJunctionModel.Models;
using APIwithoutJunctionModel.Interfaces;

namespace APIwithoutJunctionModel.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepo.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepo.GetByIdAsync(id);
        }

        public async Task<User> CreateAsync(CreateUserDTO dto)
        {
            var user = new User
            {
                userName = dto.userName,
                email = dto.email,
                password = dto.password,
                role = dto.role
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateAsync(int id, CreateUserDTO dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            user.userName = dto.userName;
            user.email = dto.email;
            user.password = dto.password;
            user.role = dto.role;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepo.DeleteAsync(id);
            await _userRepo.SaveChangesAsync();
            return true;
        }
    }
}
