using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class TeamMemberConfig : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.FullName)
            .IsRequired();

        entity
            .Property(e => e.CategoryId)
            .IsRequired();

        entity
            .Property(e => e.Priority)
            .IsRequired();

        entity
            .Property(e => e.Status)
            .IsRequired();

        entity
            .Property(e => e.Description);

        entity
            .Property(e => e.ImageId);

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<TeamMember>(e => e.ImageId);

        entity
            .Property(e => e.Email);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();

        entity
            .HasIndex(e => new { e.CategoryId, e.Priority })
            .IsUnique();
    }
}
