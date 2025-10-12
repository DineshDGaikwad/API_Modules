using System.ComponentModel.DataAnnotations.Schema;

namespace APIAssessment.Models
{
    public class BookingShow
    {
        public int Id { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        [ForeignKey("Show")]
        public int ShowId { get; set; }

        public int SeatCount { get; set; }

        public decimal SubTotal { get; set; }

        public Booking Booking { get; set; } 
        public Show Show { get; set; } 
    }
}
