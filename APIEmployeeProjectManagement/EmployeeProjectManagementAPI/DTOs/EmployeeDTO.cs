namespace EmployeeProjectAPI.DTOs
{
    public class EmployeeCreateDTO
    {
        public string EmpName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
        public int? DepartmentId { get; set; }
    }

    public class EmployeeReadDTO
    {
        public int EmployeeId { get; set; }
        public string EmpName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
    }
}
