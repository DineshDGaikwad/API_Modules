namespace APIAssessment.DTOs
{
    public class MovieCreateDto
    {
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;
        public int DurationMins { get; set; }
    }

    public class MovieResponseDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;
        public int DurationMins { get; set; }
    }
}
