using Microsoft.EntityFrameworkCore;

namespace MyMvcApp1.Models
{
    public partial class CollegeContext : DbContext
    {
        public CollegeContext(DbContextOptions<CollegeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Student Entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudId).HasName("PK_Student");
                entity.ToTable("Student");

                entity.Property(e => e.StudId).HasColumnName("stud_id");
                entity.Property(e => e.StudName)
                      .HasMaxLength(100)
                      .IsUnicode(false)
                      .HasColumnName("stud_name");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.HasOne(d => d.Dept)
                      .WithMany(p => p.Students)
                      .HasForeignKey(d => d.DeptId)
                      .HasConstraintName("FK_Student_Department");
            });

            // Department Entity
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DeptId).HasName("PK_Department");
                entity.ToTable("Department");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");
                entity.Property(e => e.DeptName)
                      .HasMaxLength(100)
                      .IsUnicode(false)
                      .HasColumnName("dept_name");
            });

            // Teacher Entity
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.TId).HasName("PK_Teacher");
                entity.ToTable("Teacher");

                entity.Property(e => e.TId).HasColumnName("t_id");
                entity.Property(e => e.TName)
                      .HasMaxLength(100)
                      .IsUnicode(false)
                      .HasColumnName("t_name");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.HasOne(d => d.Dept)
                      .WithMany(p => p.Teachers)
                      .HasForeignKey(d => d.DeptId)
                      .HasConstraintName("FK_Teacher_Department");
            });

            // Course Entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId).HasName("PK_Course");
                entity.ToTable("Course");

                entity.Property(e => e.CourseId).HasColumnName("course_id");
                entity.Property(e => e.CourseName)
                      .HasMaxLength(100)
                      .IsUnicode(false)
                      .HasColumnName("course_name");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");
                entity.Property(e => e.TId).HasColumnName("t_id");

                entity.HasOne(d => d.Dept)
                      .WithMany(p => p.Courses)
                      .HasForeignKey(d => d.DeptId)
                      .HasConstraintName("FK_Course_Department");

                entity.HasOne(d => d.TIdNavigation)
                      .WithMany(p => p.Courses)
                      .HasForeignKey(d => d.TId)
                      .HasConstraintName("FK_Course_Teacher");
            });
        }
    }
}
