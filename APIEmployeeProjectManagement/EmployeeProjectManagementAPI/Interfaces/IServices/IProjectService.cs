using EmployeeProjectAPI.DTOs;

namespace EmployeeProjectAPI.Interfaces.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectReadDTO>> GetAllAsync();
        Task<ProjectReadDTO?> GetByIdAsync(int id);
        Task<ProjectReadDTO> CreateAsync(ProjectCreateDTO dto);
        Task<bool> AssignEmployeeAsync(int projectId, int employeeId);
    }
}
