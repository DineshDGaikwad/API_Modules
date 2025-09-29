namespace APIManytoMany.Domain
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PostedOn { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<UserPost>? UserPosts { get; set; }
    }
}
