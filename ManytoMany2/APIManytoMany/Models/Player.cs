namespace APIManytoMany.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public string Address { get; set; } = null!;

        public ICollection<PlayerSport> PlayerSports { get; set; } = new List<PlayerSport>();
    }
}
