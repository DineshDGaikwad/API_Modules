using APIwithoutJunctionModel.DTOs;

namespace APIwithoutJunctionModel.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<GetPatientDTO>> GetAllAsync();
        Task<GetPatientDTO?> GetByIdAsync(int id);
        Task<GetPatientDTO> CreateAsync(CreatePatientDTO dto);
        Task<bool> UpdateAsync(int id, CreatePatientDTO dto);
        Task<bool> DeleteAsync(int id);
    }

    public class CreatePatientDTO
    {
    }

    public class GetPatientDTO
    {
    }
}
