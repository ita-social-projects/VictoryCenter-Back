using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class TeamCategoryConfig : IEntityTypeConfiguration<TeamCategory>
{
    public void Configure(EntityTypeBuilder<TeamCategory> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Name)
            .IsRequired();

        entity
            .HasIndex(e => e.Name)
            .IsUnique();

        entity
            .Property(e => e.Description);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();

        entity
            .HasMany(e => e.TeamMembers)
            .WithOne(e => e.TeamCategory);

        entity.HasMany(e => e.TeamMembers)
            .WithOne(e => e.TeamCategory)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
