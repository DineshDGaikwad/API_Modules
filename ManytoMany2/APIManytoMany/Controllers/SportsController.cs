using Microsoft.AspNetCore.Mvc;
using APIManytoMany.Models;
using APIManytoMany.DTOs;
using APIManytoMany.Repositories.Interfaces;

namespace APIManytoMany.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SportsController : ControllerBase
    {
        private readonly ISportRepository _sportRepo;

        public SportsController(ISportRepository sportRepo)
        {
            _sportRepo = sportRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sports = await _sportRepo.GetAllAsync();

            var sportDtos = sports.Select(s => new SportDto
            {
                SportId = s.SportId,
                SportName = s.SportName,
                Category = s.Category,
                OriginCountry = s.OriginCountry
            }).ToList();

            return Ok(sportDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sport = await _sportRepo.GetByIdAsync(id);
            if (sport == null) return NotFound();

            var sportDto = new SportDto
            {
                SportId = sport.SportId,
                SportName = sport.SportName,
                Category = sport.Category,
                OriginCountry = sport.OriginCountry
            };

            return Ok(sportDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Sport sport)
        {
            var newSport = await _sportRepo.AddAsync(sport);
            return CreatedAtAction(nameof(GetById), new { id = newSport.SportId }, newSport);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Sport sport)
        {
            if (id != sport.SportId) return BadRequest();
            await _sportRepo.UpdateAsync(sport);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sportRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
