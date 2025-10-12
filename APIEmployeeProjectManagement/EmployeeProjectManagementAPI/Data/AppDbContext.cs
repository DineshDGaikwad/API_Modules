using Microsoft.EntityFrameworkCore;
using EmployeeProjectManagementAPI.Models;

namespace EmployeeProjectManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectEmployee> ProjectEmployees => Set<ProjectEmployee>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);


    modelBuilder.Entity<Employee>()
        .HasOne(e => e.Department)
        .WithMany(d => d.Employees)
        .HasForeignKey(e => e.DepartmentId)
        .OnDelete(DeleteBehavior.SetNull); 

    modelBuilder.Entity<Department>()
        .HasOne(d => d.Manager)
        .WithMany() 
        .HasForeignKey(d => d.ManagerId)
        .OnDelete(DeleteBehavior.Restrict);


    modelBuilder.Entity<ProjectEmployee>()
        .HasKey(pe => new { pe.ProjectId, pe.EmployeeId });

    modelBuilder.Entity<ProjectEmployee>()
        .HasOne(pe => pe.Employee)
        .WithMany(e => e.ProjectEmployees)
        .HasForeignKey(pe => pe.EmployeeId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<ProjectEmployee>()
        .HasOne(pe => pe.Project)
        .WithMany(p => p.ProjectEmployees)
        .HasForeignKey(pe => pe.ProjectId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Project>()
        .HasOne(p => p.Manager)
        .WithMany()
        .HasForeignKey(p => p.ManagerId)
        .OnDelete(DeleteBehavior.Restrict);
}

    }
}
