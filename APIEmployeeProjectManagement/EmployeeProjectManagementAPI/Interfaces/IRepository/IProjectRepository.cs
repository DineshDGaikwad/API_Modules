using EmployeeProjectManagementAPI.Models;

namespace EmployeeProjectManagementAPI.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByProjectIdAsync(int projectId);
    }
}
