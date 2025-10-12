namespace APIAssessment.DTOs
{
    public class ShowCreateDto
    {
        public int MovieId { get; set; }
        public int ScreenId { get; set; }
        public DateTime StartsAt { get; set; }
        public decimal PricePerTicket { get; set; }
    }

    public class ShowResponseDto
    {
        public int ShowId { get; set; }
        public string MovieTitle { get; set; } = null!;
        public string ScreenName { get; set; } = null!;
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public decimal PricePerTicket { get; set; }
    }
}
