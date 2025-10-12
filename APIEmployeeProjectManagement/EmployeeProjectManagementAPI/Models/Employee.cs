namespace EmployeeProjectManagementAPI.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmpName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? DepartmentId { get; set; }

        public Department? Department { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
    }
}
