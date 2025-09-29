namespace APIManytoMany.Domain
{
    public class User
    {
        public int UsrId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<UserPost>? UserPosts { get; set; }
    }
}
