using APIPropertyRegistry.DTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIPropertyRegistry.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentResponseDto>> GetAllAsync();
        Task<IEnumerable<DocumentResponseDto>> GetPendingAsync();
        Task<IEnumerable<DocumentResponseDto>> GetByPropertyAsync(int propertyId);
        Task<DocumentResponseDto?> GetByIdAsync(int id);
        Task<DocumentResponseDto> CreateAsync(DocumentCreateDto dto, IFormFile file);
        Task<bool> VerifyAsync(DocumentVerifyDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
