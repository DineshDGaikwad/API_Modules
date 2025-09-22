using CodeFirstEmptyController.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstEmptyController.Repository
{
    public class AuthorRepository : IBookAuthor<Author>
    {
        private readonly BookAuthContext _context;

        public AuthorRepository(BookAuthContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAll()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author?> GetById(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task Add(Author entity)
        {
            await _context.Authors.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Author entity)
        {
            _context.Authors.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }
    }
}
