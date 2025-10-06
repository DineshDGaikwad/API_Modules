// using APIwithoutJunctionModel.DTOs;
// using APIwithoutJunctionModel.Interfaces;
// using APIwithoutJunctionModel.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace APIwithoutJunctionModel.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class PatientsController : ControllerBase
//     {
//         private readonly IPatientService _patientService;

//         public PatientsController(IPatientService patientService)
//         {
//             _patientService = patientService;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAll()
//         {
//             var patients = await _patientService.GetAllAsync();
//             return Ok(patients);
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetById(int id)
//         {
//             var patient = await _patientService.GetByIdAsync(id);
//             if (patient == null) return NotFound();
//             return Ok(patient);
//         }

//         [HttpPost]
//         public async Task<IActionResult> Create([FromBody] CreatePatientDTO dto)
//         {
//             var patient = await _patientService.CreateAsync(dto);
//             return CreatedAtAction(nameof(GetById), new { id = patient.PatientId }, patient);
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> Update(int id, [FromBody] CreatePatientDTO dto)
//         {
//             var updated = await _patientService.UpdateAsync(id, dto);
//             if (!updated) return NotFound();
//             return NoContent();
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var deleted = await _patientService.DeleteAsync(id);
//             if (!deleted) return NotFound();
//             return NoContent();
//         }
//     }
// }
