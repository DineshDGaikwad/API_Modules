using APIManytoMany.Application;
using APIManytoMany.Domain;
using APIManytoMany.Data;
using Microsoft.EntityFrameworkCore;

namespace APIManytoMany.Infrastructure
{
    // Interface - Repository - Service - Controller
    public class PostRepository : IUserPost<User, int>
    {
        private readonly UserPostContext _context;

        public PostRepositoryRepository(UserPostContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<Post> GetById(int id)
        {
            return await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
        }

        public async Task<Post> AddPost(Post entity)
        {
            await _context.Posts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Post> Update(int id, Post post)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing == null) return null;

        existing.PostName = post.PostName;
        existing.Title = post.Title;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> Delete(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    }
}