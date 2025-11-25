using APIPropertyRegistry.Models;
using Microsoft.EntityFrameworkCore;

namespace APIPropertyRegistry.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<PropertyOwnership> PropertyOwnerships { get; set; }
        public DbSet<PropertyTransaction> PropertyTransactions { get; set; }
        public DbSet<AgentProperty> AgentProperties { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Approver)
                .WithMany()
                .HasForeignKey(u => u.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Creator)
                .WithMany(u => u.CreatedProperties)
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedPropertyListings)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Agent)
                .WithMany(u => u.AgentPropertyListings)
                .HasForeignKey(p => p.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Verifier)
                .WithMany(u => u.VerifiedProperties)
                .HasForeignKey(p => p.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Approver)
                .WithMany()
                .HasForeignKey(p => p.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Property)
                .WithMany(p => p.Documents)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Uploader)
                .WithMany(u => u.UploadedDocuments)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Verifier)
                .WithMany()
                .HasForeignKey(d => d.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyOwnership>()
                .HasOne(o => o.Property)
                .WithMany(p => p.Ownerships)
                .HasForeignKey(o => o.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyOwnership>()
                .HasOne(o => o.User)
                .WithMany(u => u.OwnedProperties)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyOwnership>()
                .HasOne(o => o.Verifier)
                .WithMany()
                .HasForeignKey(o => o.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyTransaction>()
                .HasOne(t => t.Property)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyTransaction>()
                .HasOne(t => t.Seller)
                .WithMany(u => u.SellerTransactions)
                .HasForeignKey(t => t.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyTransaction>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.BuyerTransactions)
                .HasForeignKey(t => t.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyTransaction>()
                .HasOne(t => t.Agent)
                .WithMany(u => u.AgentTransactions)
                .HasForeignKey(t => t.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyTransaction>()
                .HasOne(t => t.Verifier)
                .WithMany()
                .HasForeignKey(t => t.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyTransaction>()
                .HasIndex(t => t.PropertyId)
                .HasFilter("[IsArchived] = 0 AND [Status] IN ('Pending','Pending Admin')")
                .IsUnique();

            modelBuilder.Entity<AgentProperty>()
                .HasKey(ap => ap.AgentPropertyId);

            modelBuilder.Entity<AgentProperty>()
                .HasOne(ap => ap.Agent)
                .WithMany()
                .HasForeignKey(ap => ap.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AgentProperty>()
                .HasOne(ap => ap.Property)
                .WithMany()
                .HasForeignKey(ap => ap.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
