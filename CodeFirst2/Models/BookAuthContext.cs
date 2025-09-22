using Microsoft.EntityFrameworkCore;

namespace CodeFirstEmptyController.Models
{
    public class BookAuthContext : DbContext
    {
        public BookAuthContext(DbContextOptions<BookAuthContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
            .HasData(
                new Author() { AuthId = 11, AuthName = "Bravo" },
                new Author() { AuthId = 12, AuthName = "Marco" }
            );

            modelBuilder.Entity<Book>()
            .HasData(
                new Book() { BookId = 1, Title = "Dj Bravoooo", Price = 4000, PublicationYear = new DateOnly(2020, 8, 12), AuthId = 11 },
                new Book() { BookId = 2, Title = "The Devilll", Price = 11111, PublicationYear = new DateOnly(2023, 3, 30), AuthId = 12 }
            );
        }
    }
}