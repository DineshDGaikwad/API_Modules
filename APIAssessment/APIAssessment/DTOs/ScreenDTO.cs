namespace APIAssessment.DTOs
{
    public class ScreenCreateDto
    {
        public string Name { get; set; } = null!;
        public string Theater { get; set; } = null!;
        public int Capacity { get; set; }
    }

    public class ScreenResponseDto
    {
        public int ScreenId { get; set; }
        public string Name { get; set; } = null!;
        public string Theater { get; set; } = null!;
        public int Capacity { get; set; }
    }
}
