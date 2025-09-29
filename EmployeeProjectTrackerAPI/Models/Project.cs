using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeProjectTrackerAPI.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required, StringLength(10)]
        public string ProjectCode { get; set; } = null!;

        [Required, StringLength(100)]
        public string ProjectName { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public decimal Budget { get; set; }

        // Navigation property to Employees
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
