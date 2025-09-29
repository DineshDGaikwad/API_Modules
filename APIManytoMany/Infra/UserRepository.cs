using APIManytoMany.Application;
using APIManytoMany.Domain;
using APIManytoMany.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra
{
    // Interface - Repository - Service - Controller
    public class UserRepository : IUserPost<User, int>
    {
        private readonly UserPostContext _context;

        public UserRepository(UserPostContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UsrId == id);
        }

        public async Task<User> AddUser(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<User> Update(int id, User user)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing == null) return null;

        existing.UserName = user.UserName;
        existing.Email = user.Email;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    }
}