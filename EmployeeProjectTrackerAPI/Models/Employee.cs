using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeProjectTrackerAPI.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required, StringLength(8)]
        public string EmployeeCode { get; set; } = null!;

        [Required, StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, StringLength(50)]
        public string Designation { get; set; } = null!;

        [Required]
        public decimal Salary { get; set; }

        // Foreign key
        public int ProjectId { get; set; }

        // Navigation property
        [JsonIgnore] // Only include when using Include in GET queries
        public Project? Project { get; set; }
    }
}
