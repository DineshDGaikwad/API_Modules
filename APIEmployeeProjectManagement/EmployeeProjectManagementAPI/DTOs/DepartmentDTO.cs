namespace EmployeeProjectAPI.DTOs
{
    public class DepartmentCreateDTO
    {
        public string DeptName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
    }

    public class DepartmentReadDTO
    {
        public int DepartmentId { get; set; }
        public string DeptName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
    }
}
