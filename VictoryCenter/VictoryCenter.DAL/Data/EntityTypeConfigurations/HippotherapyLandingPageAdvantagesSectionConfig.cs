using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageAdvantagesSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageAdvantagesSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageAdvantagesSection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.HippotherapyLandingPageId)
            .IsRequired();

        entity.Property(e => e.Title)
            .IsRequired();

        entity.HasMany(e => e.AdvantageCards)
            .WithOne(c => c.AdvantagesSection)
            .HasForeignKey(c => c.AdvantagesSectionId);

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
