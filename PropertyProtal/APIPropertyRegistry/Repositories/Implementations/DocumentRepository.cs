using APIPropertyRegistry.Data;
using APIPropertyRegistry.Models;
using APIPropertyRegistry.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .Include(d => d.Property)
                .Include(d => d.Uploader)
                .Include(d => d.Verifier)
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetPendingAsync()
        {
            return await _context.Documents
                .Include(d => d.Property)
                .Include(d => d.Uploader)
                .Where(d => !d.Verified)
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetByPropertyAsync(int propertyId)
        {
            return await _context.Documents
                .Include(d => d.Property)
                .Include(d => d.Uploader)
                .Include(d => d.Verifier)
                .Where(d => d.PropertyId == propertyId)
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.Property)
                .Include(d => d.Uploader)
                .Include(d => d.Verifier)
                .FirstOrDefaultAsync(d => d.DocumentId == id);
        }

        public async Task<Document> AddAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return false;

            _context.Documents.Remove(doc);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
