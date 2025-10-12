using Microsoft.AspNetCore.Mvc;
using APIAssessment.DTOs;
using APIAssessment.Interfaces;
using APIAssessment.Models;
using APIAssessment.Data;

namespace APIAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _service;
        private readonly AppDbContext _context;

        public ShowsController(IShowService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime date)
        {
            var shows = await _service.GetShowsByDateAsync(date);
            return Ok(shows);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ShowCreateDto dto)
        {
            var movie = await _context.Movies.FindAsync(dto.MovieId);
            if (movie == null) return BadRequest("Movie not found");

            var show = new Show
            {
                MovieId = dto.MovieId,
                ScreenId = dto.ScreenId,
                StartsAt = dto.StartsAt,
                EndsAt = dto.StartsAt.AddMinutes(movie.DurationMins),
                PricePerTicket = dto.PricePerTicket
            };

            var success = await _service.AddShowAsync(show);
            if (!success) return BadRequest("Overlapping show on the same screen");

            return Ok(show);
        }
    }
}
