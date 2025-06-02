using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data;

public class VictoryCenterDbContext : DbContext
{
    public VictoryCenterDbContext(DbContextOptions<VictoryCenterDbContext> options) 
        : base(options) {}
    
    public DbSet<Admin> Admins { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<TeamMember> TeamMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TeamMember>()
                .HasOne(t => t.Category)
                .WithMany(c => c.TeamMembers)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeamMember>()
            .HasIndex(t => new { t.CategoryId, t.Priority })
            .IsUnique();
    }
}
