using EmployeeProjectManagementAPI.Data;
using EmployeeProjectManagementAPI.Interfaces.Repositories;
using EmployeeProjectManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectManagementAPI.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;
        public DepartmentRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Department>> GetAllAsync() =>
            await _context.Departments.Include(d => d.ManagerId).ToListAsync();

        public async Task<Department?> GetByIdAsync(int id) =>
            await _context.Departments.Include(d => d.ManagerId)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);

        public async Task AddAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept != null)
            {
                _context.Departments.Remove(dept);
                await _context.SaveChangesAsync();
            }
        }
    }
}
