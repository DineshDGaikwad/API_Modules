using System.ComponentModel.DataAnnotations;

namespace APIAssessment.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Language { get; set; } = null!;

        [Range(30, int.MaxValue)]
        public int DurationMins { get; set; }

        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
