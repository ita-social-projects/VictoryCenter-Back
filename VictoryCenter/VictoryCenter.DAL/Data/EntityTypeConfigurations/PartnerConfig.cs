using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

public class PartnerConfig : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> entity)
    {
        entity
            .HasKey(e => e.Id);

        entity
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity
            .Property(e => e.Description)
            .IsRequired();

        entity
            .Property(e => e.PartnersSectionId)
            .IsRequired();

        entity
            .Property(e => e.Priority)
            .IsRequired();

        entity
            .Property(e => e.Description);

        entity
            .Property(e => e.ImageId);

        entity
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<Partner>(e => e.ImageId);

        entity
            .Property(e => e.CreatedAt)
            .IsRequired();

        entity
            .HasIndex(e => new { e.PartnersSectionId, e.Priority })
            .IsUnique();
    }
}
