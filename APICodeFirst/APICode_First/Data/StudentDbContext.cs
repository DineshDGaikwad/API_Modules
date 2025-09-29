using Microsoft.EntityFrameworkCore;
using CodeFirstAPI.Models;

namespace CodeFirstAPI.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!; // null-forgiving

        // Seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, FullName = "Ravi Kumar", Age = 22, Email = "ravi@example.com", Course = "Computer Science" },
                new Student { StudentId = 2, FullName = "Neha Sharma", Age = 21, Email = "neha@example.com", Course = "Electronics" }
            );
        }
    }
}
