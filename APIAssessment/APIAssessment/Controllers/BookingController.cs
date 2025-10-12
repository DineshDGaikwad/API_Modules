using APIAssessment.Interfaces;
using Microsoft.AspNetCore.Mvc;
using APIAssessment.Models;
using APIAssessment.DTOs;

namespace APIAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;
        public BookingsController(IBookingService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _service.GetAllAsync();
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Post(BookingCreateDto dto)
        {
            var booking = new Booking { UserId = dto.UserId };
            var bookingShows = dto.Shows
                .Select(s => new BookingShow { ShowId = s.ShowId, SeatCount = s.SeatCount })
                .ToList();

            if (await _service.AddBookingAsync(booking, bookingShows))
                return Ok(booking);

            return BadRequest("Booking failed due to seat limit or invalid show.");
        }
    }
}