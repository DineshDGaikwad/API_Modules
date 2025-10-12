namespace EmployeeProjectAPI.DTOs
{
    public class ProjectCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ProjectReadDTO
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string>? EmployeeNames { get; set; }
    }
}
