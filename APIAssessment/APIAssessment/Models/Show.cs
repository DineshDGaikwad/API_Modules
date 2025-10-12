using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAssessment.Models
{
    public class Show
    {
        public int ShowId { get; set; }

        [ForeignKey("Movie")]
        public int MovieId { get; set; }

        [ForeignKey("Screen")]
        public int ScreenId { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerTicket { get; set; }

        public Movie Movie { get; set; }
        public Screen Screen { get; set; } 
        public ICollection<BookingShow> BookingShows { get; set; } = new List<BookingShow>();
    }
}
