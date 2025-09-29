namespace APIManytoMany.Models
{
    public class Sport
    {
        public int SportId { get; set; }
        public string SportName { get; set; } = null!;
        public string Category { get; set; } = null!;      // Indoor/Outdoor
        public string OriginCountry { get; set; } = null!;

        public ICollection<PlayerSport> PlayerSports { get; set; } = new List<PlayerSport>();
    }
}
