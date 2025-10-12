using EmployeeProjectAPI.DTOs;
using EmployeeProjectAPI.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _service.GetAllAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _service.GetByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ProjectId }, created);
        }

        [HttpPost("{projectId}/assign/{employeeId}")]
        public async Task<IActionResult> AssignEmployee(int projectId, int employeeId)
        {
            var success = await _service.AssignEmployeeAsync(projectId, employeeId);
            if (!success) return NotFound();
            return Ok("Employee assigned successfully.");
        }
    }
}
