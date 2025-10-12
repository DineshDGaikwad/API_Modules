using EmployeeProjectAPI.DTOs;
using EmployeeProjectAPI.Interfaces.Services;
using EmployeeProjectManagementAPI.Interfaces.Repositories;
using EmployeeProjectManagementAPI.Models;

namespace EmployeeProjectAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IDepartmentRepository _deptRepo;

        public EmployeeService(IEmployeeRepository repo, IDepartmentRepository deptRepo)
        {
            _repo = repo;
            _deptRepo = deptRepo;
        }

        public async Task<IEnumerable<EmployeeReadDTO>> GetAllAsync()
        {
            var employees = await _repo.GetAllAsync();
            return employees.Select(e => new EmployeeReadDTO
            {
                EmployeeId = e.EmployeeId,
                EmpName = e.EmpName,
                Email = e.Email,
                Role = e.Role,
                DepartmentName = e.Department?.DeptName
            });
        }

        public async Task<EmployeeReadDTO?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;

            return new EmployeeReadDTO
            {
                EmployeeId = e.EmployeeId,
                EmpName = e.EmpName,
                Email = e.Email,
                Role = e.Role,
                DepartmentName = e.Department?.DeptName
            };
        }

        public async Task<EmployeeReadDTO> CreateAsync(EmployeeCreateDTO dto)
        {
            var employee = new Employee
            {
                EmpName = dto.EmpName,
                Email = dto.Email,
                Role = dto.Role,
                DepartmentId = dto.DepartmentId
            };
            await _repo.AddAsync(employee);

            return new EmployeeReadDTO
            {
                EmployeeId = employee.EmployeeId,
                EmpName = employee.EmpName,
                Email = employee.Email,
                Role = employee.Role
            };
        }

        public async Task<bool> UpdateAsync(int id, EmployeeCreateDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.EmpName = dto.EmpName;
            existing.Email = dto.Email;
            existing.Role = dto.Role;
            existing.DepartmentId = dto.DepartmentId;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
