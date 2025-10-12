using EmployeeProjectAPI.DTOs;
using EmployeeProjectAPI.Interfaces.Services;
using EmployeeProjectManagementAPI.Interfaces.Repositories;
using EmployeeProjectManagementAPI.Models;
namespace EmployeeProjectAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;

        public DepartmentService(IDepartmentRepository repo, IEmployeeRepository employeeRepo)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
        }

        public async Task<IEnumerable<DepartmentReadDTO>> GetAllAsync()
        {
            var departments = await _repo.GetAllAsync();
            var departmentDTOs = await Task.WhenAll(departments.Select(async d => new DepartmentReadDTO
            {
                DepartmentId = d.DepartmentId,
                DeptName = d.DeptName,
                Location = d.Location,
                ManagerName = d.ManagerId.HasValue
                    ? (await _employeeRepo.GetByIdAsync(d.ManagerId.Value))?.EmpName
                    : null
            }));
            return departmentDTOs;
        }

        public async Task<DepartmentReadDTO?> GetByIdAsync(int id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return null;

            string? managerName = null;
            if (d.ManagerId.HasValue)
            {
                var manager = await _employeeRepo.GetByIdAsync(d.ManagerId.Value);
                managerName = manager?.EmpName;
            }

            return new DepartmentReadDTO
            {
                DepartmentId = d.DepartmentId,
                DeptName = d.DeptName,
                Location = d.Location,
                ManagerName = managerName
            };
        }

        public async Task<DepartmentReadDTO> CreateAsync(DepartmentCreateDTO dto)
        {
            var department = new Department
            {
                DeptName = dto.DeptName,
                Location = dto.Location,
                ManagerId = dto.ManagerId
            };
            await _repo.AddAsync(department);

            return new DepartmentReadDTO
            {
                DepartmentId = department.DepartmentId,
                DeptName = department.DeptName,
                Location = department.Location
            };
        }

        public async Task<bool> AssignManagerAsync(int departmentId, int employeeId)
        {
            var dept = await _repo.GetByIdAsync(departmentId);
            var emp = await _employeeRepo.GetByIdAsync(employeeId);

            if (dept == null || emp == null) return false;

            dept.ManagerId = emp.EmployeeId;
            await _repo.UpdateAsync(dept);
            return true;
        }
    }
}
