using Microsoft.AspNetCore.Mvc;
using APIAssessment.Models;
using APIAssessment.Services;
using APIAssessment.DTOs;
using APIAssessment.Interfaces;

namespace APIAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _service;
        public MovieController(IMovieService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Post(MovieCreateDto dto)
        {
            var movie = new Movie
            {
                Title = dto.Title,
                Language = dto.Language,
                DurationMins = dto.DurationMins
            };
            if (await _service.AddAsync(movie)) return Ok(movie);
            return BadRequest();
        }
    }
}
