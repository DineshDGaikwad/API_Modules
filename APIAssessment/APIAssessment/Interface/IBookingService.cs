using APIAssessment.Models;

namespace APIAssessment.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<bool> AddBookingAsync(Booking booking, List<BookingShow> bookingShows);
    }
}
