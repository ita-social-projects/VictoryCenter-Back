using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageHippoventionCenterSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageHippoventionCenterSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageHippoventionCenterSection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.HippotherapyLandingPageId)
            .IsRequired();

        entity.Property(e => e.Title)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();

        entity.Property(e => e.ImageId);

        entity.HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<HippotherapyLandingPageHippoventionCenterSection>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasMany(e => e.HippoventionPros)
            .WithOne(p => p.HippoventionCenterSection)
            .HasForeignKey(p => p.HippoventionCenterSectionId);

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
