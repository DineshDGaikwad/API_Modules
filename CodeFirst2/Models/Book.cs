using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstEmptyController.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateOnly PublicationYear { get; set; }

        public int AuthId { get; set; }

        // Navigation Property
        [ForeignKey("AuthId")]
        public Author? author { get; set; }

    }
}