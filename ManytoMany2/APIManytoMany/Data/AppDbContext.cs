using Microsoft.EntityFrameworkCore;
using APIManytoMany.Models;

namespace APIManytoMany.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<PlayerSport> PlayerSports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerSport>()
                .HasKey(ps => new { ps.PlayerId, ps.SportId });

            modelBuilder.Entity<PlayerSport>()
                .HasOne(ps => ps.Player)
                .WithMany(p => p.PlayerSports)
                .HasForeignKey(ps => ps.PlayerId);

            modelBuilder.Entity<PlayerSport>()
                .HasOne(ps => ps.Sport)
                .WithMany(s => s.PlayerSports)
                .HasForeignKey(ps => ps.SportId);
        }
    }
}
