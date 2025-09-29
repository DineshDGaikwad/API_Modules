namespace APIManytoMany.DTOs
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public string Address { get; set; } = null!;
        
        public List<PlayerSportDto> Sports { get; set; } = new List<PlayerSportDto>();
    }
}
