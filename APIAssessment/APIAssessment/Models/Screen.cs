using System.ComponentModel.DataAnnotations;

namespace APIAssessment.Models
{
    public class Screen
    {
        public int ScreenId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Theater { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
