using EmployeeProjectAPI.DTOs;

namespace EmployeeProjectAPI.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentReadDTO>> GetAllAsync();
        Task<DepartmentReadDTO?> GetByIdAsync(int id);
        Task<DepartmentReadDTO> CreateAsync(DepartmentCreateDTO dto);
        Task<bool> AssignManagerAsync(int departmentId, int employeeId);
    }
}
