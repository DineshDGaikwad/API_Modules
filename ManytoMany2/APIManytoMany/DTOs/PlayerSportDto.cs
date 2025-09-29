namespace APIManytoMany.DTOs
{
    public class PlayerSportDto
    {
        public int SportId { get; set; }
        public string SportName { get; set; } = null!;
        public int YearsPlayed { get; set; }
        public string Position { get; set; } = null!;
    }
}
