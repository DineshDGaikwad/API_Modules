namespace APIManytoMany.Domain
{
    public class UserPost
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;  // Navigation to User

        public int PostId { get; set; }
        public Post Post { get; set; } = null!; // Navigation to Post

        public bool IsAuthor { get; set; }
    }
}
