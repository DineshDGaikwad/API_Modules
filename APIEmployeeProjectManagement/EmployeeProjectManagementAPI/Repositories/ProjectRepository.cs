using EmployeeProjectManagementAPI.Data;
using EmployeeProjectManagementAPI.Interfaces.Repositories;
using EmployeeProjectManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectManagementAPI.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Project>> GetAllAsync() =>
            await _context.Projects.Include(p => p.Manager).ToListAsync();

        public async Task<Project?> GetByIdAsync(int id) =>
            await _context.Projects.Include(p => p.Manager)
            .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employee)
            .FirstOrDefaultAsync(p => p.ProjectId == id);

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByProjectIdAsync(int projectId)
        {
            return await _context.ProjectEmployees
                .Where(pe => pe.ProjectId == projectId)
                .Select(pe => pe.Employee)
                .ToListAsync();
        }
    }
}
