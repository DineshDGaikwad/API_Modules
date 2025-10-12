using EmployeeProjectAPI.DTOs;
using EmployeeProjectAPI.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentsController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _service.GetAllAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _service.GetByIdAsync(id);
            if (department == null) return NotFound();
            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentCreateDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.DepartmentId }, created);
        }

        [HttpPut("{id}/assign-manager/{employeeId}")]
        public async Task<IActionResult> AssignManager(int id, int employeeId)
        {
            var success = await _service.AssignManagerAsync(id, employeeId);
            if (!success) return NotFound();
            return Ok("Manager assigned successfully.");
        }
    }
}
