using APIwithoutJunctionModel.DTOs;
using APIwithoutJunctionModel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIwithoutJunctionModel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocDTO dto)
        {
            var doctor = await _doctorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = doctor.DoctorId }, doctor);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateDocDTO dto)
        {
            var updated = await _doctorService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _doctorService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
