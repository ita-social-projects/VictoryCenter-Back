using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data;

public class VictoryCenterDbContext : DbContext
{
    public VictoryCenterDbContext(DbContextOptions<VictoryCenterDbContext> options) 
        : base(options) {}

    public DbSet<TestEntity> TestEntities { get; set; }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<TeamMember> TeamMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.ToTable("test_entities");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TestName)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserName)
                .IsRequired();

            entity.Property(e => e.Password)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired();

            entity.Property(e => e.Description);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasMany(e => e.TeamMembers)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FirstName)
                .IsRequired();

            entity.Property(e => e.LastName)
                .IsRequired();

            entity.Property(e => e.MiddleName);

            entity.Property(e => e.CategoryId)
                .IsRequired();

            entity.Property(e => e.Priority)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired();

            entity.Property(e => e.Description);

            entity.Property(e => e.Photo);

            entity.Property(e => e.Email);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasIndex(e => new { e.CategoryId, e.Priority })
                .IsUnique();
        });
    }
}
