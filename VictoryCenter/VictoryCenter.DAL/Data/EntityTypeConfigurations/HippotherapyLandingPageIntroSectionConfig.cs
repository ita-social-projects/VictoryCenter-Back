using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageIntroSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageIntroSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageIntroSection> entity)
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
            .HasForeignKey<HippotherapyLandingPageIntroSection>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
