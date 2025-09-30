using APIwithoutJunctionModel.DTOs;

namespace APIwithoutJunctionModel.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<GetDocDTO>> GetAllAsync();
        Task<GetDocDTO?> GetByIdAsync(int id);
        Task<GetDocDTO> CreateAsync(CreateDocDTO dto);
        Task<bool> UpdateAsync(int id, CreateDocDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
