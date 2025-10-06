using APIJobPortal.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace APIJobPortal.Data
{
    public class JobPortalDbContext : DbContext
    {
        public JobPortalDbContext(DbContextOptions<JobPortalDbContext> options)
            : base(options)
        {
        }

        // 📚 DbSets
        public DbSet<JobSeeker> JobSeekers { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<JobApplication> JobApplications { get; set; } = null!;
        public DbSet<CompanyJob> CompanyJobs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Configure decimal precision for Salary
            modelBuilder.Entity<Job>()
                .Property(j => j.Salary)
                .HasColumnType("decimal(18,2)");

            // 🔗 JobApplication (Many-to-Many: Job ↔ JobSeeker)
            modelBuilder.Entity<JobApplication>()
                .HasKey(ja => new { ja.JobId, ja.JobSeekerId });

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Job)
                .WithMany(j => j.JobApplications)
                .HasForeignKey(ja => ja.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.JobSeeker)
                .WithMany(js => js.JobApplications)
                .HasForeignKey(ja => ja.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔗 CompanyJob (Many-to-Many: Company ↔ Job)
            modelBuilder.Entity<CompanyJob>()
                .HasKey(cj => new { cj.CompanyId, cj.JobId });

            modelBuilder.Entity<CompanyJob>()
                .HasOne(cj => cj.Company)
                .WithMany(c => c.CompanyJobs)
                .HasForeignKey(cj => cj.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyJob>()
                .HasOne(cj => cj.Job)
                .WithMany(j => j.CompanyJobs)
                .HasForeignKey(cj => cj.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Seed data with static values
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    CompanyId = 1,
                    Name = "TechNova Pvt Ltd",
                    Industry = "IT Services",
                    Location = "Pune",
                    Email = "contact@technova.com",
                    ContactNumber = "1234567890",
                    Website = "https://www.technova.com",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Company
                {
                    CompanyId = 2,
                    Name = "HealthLink",
                    Industry = "Healthcare",
                    Location = "Bangalore",
                    Email = "info@healthlink.com",
                    ContactNumber = "9876543210",
                    Website = "https://www.healthlink.com",
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );

            modelBuilder.Entity<Job>().HasData(
                new Job
                {
                    JobId = 1,
                    Title = "Software Developer",
                    Description = "Full Stack Developer",
                    Salary = 85000m,
                    Location = "Remote",
                    Type = "Full-Time",
                    PostedDate = new DateTime(2025, 1, 1)
                },
                new Job
                {
                    JobId = 2,
                    Title = "Data Analyst",
                    Description = "Analyze large datasets",
                    Salary = 65000m,
                    Location = "Mumbai",
                    Type = "Full-Time",
                    PostedDate = new DateTime(2025, 1, 1)
                }
            );

            modelBuilder.Entity<JobSeeker>().HasData(
                new JobSeeker
                {
                    JobSeekerId = 1,
                    FullName = "Alice Johnson",
                    Email = "alice@example.com",
                    PasswordHash = "hashedpassword1",
                    RegisteredAt = new DateTime(2025, 1, 1)
                },
                new JobSeeker
                {
                    JobSeekerId = 2,
                    FullName = "Bob Smith",
                    Email = "bob@example.com",
                    PasswordHash = "hashedpassword2",
                    RegisteredAt = new DateTime(2025, 1, 1)
                }
            );
        }
    }
}
