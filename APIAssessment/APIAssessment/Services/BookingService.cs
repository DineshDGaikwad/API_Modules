using APIAssessment.Data;
using APIAssessment.Models;
using APIAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APIAssessment.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        public BookingService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Booking>> GetAllAsync() =>
            await _context.Bookings
                .Include(b => b.BookingShows)
                .ThenInclude(bs => bs.Show)
                .ToListAsync();

        public async Task<bool> AddBookingAsync(Booking booking, List<BookingShow> bookingShows)
        {
            decimal totalAmount = 0;

            foreach (var bs in bookingShows)
            {
                var show = await _context.Shows.Include(s => s.Screen)
                                               .FirstOrDefaultAsync(s => s.ShowId == bs.ShowId);
                if (show == null) return false;

                int totalSeatsBooked = await _context.BookingShows
                    .Where(x => x.ShowId == bs.ShowId)
                    .SumAsync(x => x.SeatCount);

                if (totalSeatsBooked + bs.SeatCount > show.Screen.Capacity) return false;

                bs.SubTotal = bs.SeatCount * show.PricePerTicket;
                totalAmount += bs.SubTotal;
            }

            booking.TotalAmount = totalAmount;
            booking.BookingDate = DateTime.Now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var bs in bookingShows)
            {
                bs.BookingId = booking.BookingId;
            }

            _context.BookingShows.AddRange(bookingShows);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
