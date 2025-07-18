using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data;

public class VictoryCenterDbContext : IdentityDbContext<AdminUser, IdentityRole<int>, int>
{
    public VictoryCenterDbContext(DbContextOptions<VictoryCenterDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<VisitorPage> VisitorPages { get; set; }

    public DbSet<FaqPlacement> FaqPlacements { get; set; }

    public DbSet<FaqQuestion> FaqQuestions { get; set; }

    public DbSet<TeamMember> TeamMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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

        modelBuilder.Entity<VisitorPage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Slug)
                .IsRequired();

            entity.HasIndex(e => e.Slug)
                .IsUnique();

            entity.Property(e => e.Title)
                .IsRequired();

            entity.HasIndex(e => e.Title)
                .IsUnique();

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<FaqPlacement>(entity =>
        {
            entity.HasKey(e => new { e.PageId, e.QuestionId });

            entity.HasOne(e => e.Page)
                .WithMany(e => e.Questions)
                .HasForeignKey(e => e.PageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Question)
                .WithMany(e => e.Placements)
                .HasForeignKey(e => e.PageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.PageId, e.Priority })
                .IsUnique();
        });

        modelBuilder.Entity<FaqQuestion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.QuestionText)
                .IsRequired();

            entity.Property(e => e.AnswerText)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FullName)
                .IsRequired();

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
