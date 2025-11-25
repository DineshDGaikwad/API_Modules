using APIPropertyRegistry.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Repositories.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<IEnumerable<Document>> GetPendingAsync();
        Task<IEnumerable<Document>> GetByPropertyAsync(int propertyId);
        Task<Document?> GetByIdAsync(int id);
        Task<Document> AddAsync(Document document);
        Task<bool> UpdateAsync(Document document);
        Task<bool> DeleteAsync(int id);
    }
}
