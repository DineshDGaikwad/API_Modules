using Microsoft.EntityFrameworkCore;
using APIAssessment.Models;

namespace APIAssessment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingShow> BookingShows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
                .HasIndex(m => new { m.Title, m.Language })
                .IsUnique();

            modelBuilder.Entity<Screen>()
                .HasIndex(s => new { s.Name, s.Theater })
                .IsUnique();

            modelBuilder.Entity<BookingShow>()
                .HasIndex(bs => new { bs.BookingId, bs.ShowId })
                .IsUnique();

            modelBuilder.Entity<Show>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Shows)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Show>()
                .HasOne(s => s.Screen)
                .WithMany(sc => sc.Shows)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingShow>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingShows)
                .HasForeignKey(bs => bs.BookingId);

            modelBuilder.Entity<BookingShow>()
                .HasOne(bs => bs.Show)
                .WithMany(s => s.BookingShows)
                .HasForeignKey(bs=> bs.ShowId);
        }
    }
}