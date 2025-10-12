using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeProjectManagementAPI.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DeptName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public Employee? Manager { get; set; }  
        

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
