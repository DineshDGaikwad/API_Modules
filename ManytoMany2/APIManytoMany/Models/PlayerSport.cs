namespace APIManytoMany.Models
{
    public class PlayerSport
    {
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public int SportId { get; set; }
        public Sport Sport { get; set; } = null!;

        public int YearsPlayed { get; set; }
        public string Position { get; set; } = null!;
    }
}
