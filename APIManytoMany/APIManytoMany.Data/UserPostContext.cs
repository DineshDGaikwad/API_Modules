using APIManytoMany.Domain;
using Microsoft.EntityFrameworkCore;

namespace APIManytoMany.Data
{
    public class UserPostContext : DbContext
    {
        
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<UserPost> UserPosts { get; set; }
        
        public UserPostContext(DbContextOptions<UserPostContext> options)
            : base(options)
        {
        }


        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlServer(
        //         "Server=localhost,1433;Database=ManytoMany;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;"
        //     );
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(x => x.UsrId);
                u.Property(x => x.UserName).IsRequired().HasMaxLength(50);
                u.HasIndex(x => x.Email).IsUnique();
            });

            // Post
            modelBuilder.Entity<Post>(p =>
            {
                p.HasKey(x => x.PostId);
                p.Property(x => x.Title).IsRequired().HasMaxLength(100);
                p.Property(x => x.Content).IsRequired().HasMaxLength(400);
            });

            // UserPost (Join Table)
            modelBuilder.Entity<UserPost>(up =>
            {
                up.HasKey(x => new { x.UserId, x.PostId });

                up.HasOne(x => x.User)
                    .WithMany(u => u.UserPosts)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                up.HasOne(x => x.Post)
                    .WithMany(p => p.UserPosts)
                    .HasForeignKey(x => x.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                up.Property(x => x.IsAuthor).HasDefaultValue(false);
            });



        }
    }
}
