using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageHippoventionSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageHippoventionSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageHippoventionSection> entity)
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

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
