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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(VictoryCenterDbContext).Assembly);
    }
}
