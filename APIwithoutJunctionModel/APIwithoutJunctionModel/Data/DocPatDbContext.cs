using Microsoft.EntityFrameworkCore;
using APIwithoutJunctionModel.Models;

namespace APIwithoutJunctionModel.Data
{
    public class DocPatDbContext : DbContext
    {
        public DocPatDbContext(DbContextOptions<DocPatDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Patients)
                .WithMany(p => p.Doctors)
                .UsingEntity(j => j.ToTable("DoctorPatients")); // junction table auto-created
        }
    }
}
