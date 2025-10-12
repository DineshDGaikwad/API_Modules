using System.ComponentModel.DataAnnotations;

namespace APIAssessment.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public ICollection<BookingShow> BookingShows { get; set; } = new List<BookingShow>();
    }
}
