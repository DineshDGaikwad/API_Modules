using System.ComponentModel.DataAnnotations;

namespace CodeFirstEmptyController.Models
{
    public class Author
    {
        [Key]
        public int AuthId { get; set; }
        
        [Required(ErrorMessage = "Author name is required")]
        public string AuthName { get; set; } = string.Empty;

        public ICollection<Book>? Books { get; set; }
    }
}