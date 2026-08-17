using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageAdvantageCardConfig : IEntityTypeConfiguration<HippotherapyLandingPageAdvantageCard>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageAdvantageCard> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.AdvantagesSectionId)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();

        entity.Property(e => e.ImageId);

        entity.HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<HippotherapyLandingPageAdvantageCard>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.Property(e => e.Priority)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => new { e.AdvantagesSectionId, e.Priority })
            .IsUnique();
    }
}
