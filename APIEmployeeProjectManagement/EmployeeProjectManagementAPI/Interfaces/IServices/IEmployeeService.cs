using EmployeeProjectAPI.DTOs;

namespace EmployeeProjectAPI.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeReadDTO>> GetAllAsync();
        Task<EmployeeReadDTO?> GetByIdAsync(int id);
        Task<EmployeeReadDTO> CreateAsync(EmployeeCreateDTO dto);
        Task<bool> UpdateAsync(int id, EmployeeCreateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
