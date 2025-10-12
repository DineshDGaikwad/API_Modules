namespace APIAssessment.DTOs
{
    public class BookingCreateDto
    {
        public int UserId { get; set; }
        public List<BookingShowCreateDto> Shows { get; set; } = new();
    }

    public class BookingShowCreateDto
    {
        public int ShowId { get; set; }
        public int SeatCount { get; set; }
    }

    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
