using System.ComponentModel.DataAnnotations;

namespace CodeFirstAPI.Models
{
    public class Student
    {
        [Key] 
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Range(18, 60)]
        public int Age { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Course { get; set; } = string.Empty;
    }
}
