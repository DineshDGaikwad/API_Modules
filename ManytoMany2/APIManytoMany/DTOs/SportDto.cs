namespace APIManytoMany.DTOs
{
    public class SportDto
    {
        public int SportId { get; set; }
        public string SportName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string OriginCountry { get; set; } = null!;
    }
}
