using CMCS3.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Claim entity
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(c => c.Id); // Primary Key
                entity.Property(c => c.LecturerName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(c => c.HoursWorked)
                    .IsRequired();
                entity.Property(c => c.HourlyRate)
                    .IsRequired()
                    .HasColumnType("decimal(18, 2)");
                entity.Property(c => c.TotalAmount)
                    .HasColumnType("decimal(18, 2)");
                entity.Property(c => c.Status)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(c => c.DateSubmitted)
                    .IsRequired();

                entity.ToTable("Claims"); // Map to 'Claims' table
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId); // Primary Key
                entity.Property(r => r.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);

                // Seed initial data
                entity.HasData(
                    new Role { RoleId = 1, RoleName = "Coordinator" },
                    new Role { RoleId = 2, RoleName = "Academic Manager" }
                );
            });

            // Configure UserRole entity
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => ur.UserRoleId); // Primary Key
                entity.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
