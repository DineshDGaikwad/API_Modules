namespace EmployeeProjectManagementAPI.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ManagerId { get; set; }

        public Employee? Manager { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
        public ICollection<Employee> Employees => ProjectEmployees.Select(pe => pe.Employee).ToList();
    }
}
