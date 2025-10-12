using EmployeeProjectManagementAPI.Data;
using EmployeeProjectManagementAPI.Interfaces.Repositories;
using EmployeeProjectManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProjectManagementAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Employee>> GetAllAsync() =>
            await _context.Employees.Include(e => e.Department).ToListAsync();

        public async Task<Employee?> GetByIdAsync(int id) =>
            await _context.Employees.Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp != null)
            {
                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _context.Employees.AnyAsync(e => e.EmployeeId == id);
    }
}
