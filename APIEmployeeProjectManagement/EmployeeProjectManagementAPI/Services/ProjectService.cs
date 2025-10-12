using EmployeeProjectAPI.DTOs;
using EmployeeProjectAPI.Interfaces.Services;
using EmployeeProjectManagementAPI.Interfaces.Repositories;
using EmployeeProjectManagementAPI.Models;

namespace EmployeeProjectAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;

        public ProjectService(IProjectRepository repo, IEmployeeRepository employeeRepo)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
        }

        public async Task<IEnumerable<ProjectReadDTO>> GetAllAsync()
        {
            var projects = await _repo.GetAllAsync();
            return projects.Select(p => new ProjectReadDTO
            {
                ProjectId = p.ProjectId,
                Title = p.Title,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                EmployeeNames = p.Employees.Select(e => e.EmpName).ToList()
            });
        }

        public async Task<ProjectReadDTO?> GetByIdAsync(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;

            return new ProjectReadDTO
            {
                ProjectId = p.ProjectId,
                Title = p.Title,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                EmployeeNames = p.Employees.Select(e => e.EmpName).ToList()
            };
        }

        public async Task<ProjectReadDTO> CreateAsync(ProjectCreateDTO dto)
        {
            var project = new Project
            {
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            await _repo.AddAsync(project);

            return new ProjectReadDTO
            {
                ProjectId = project.ProjectId,
                Title = project.Title,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            };
        }

        public async Task<bool> AssignEmployeeAsync(int projectId, int employeeId)
        {
            var project = await _repo.GetByIdAsync(projectId);
            var employee = await _employeeRepo.GetByIdAsync(employeeId);

            if (project == null || employee == null) return false;

            project.Employees.Add(employee);
            await _repo.UpdateAsync(project);
            return true;
        }
    }
}
