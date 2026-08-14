using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageScientificReferencesSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageScientificReferencesSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageScientificReferencesSection> entity)
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

        entity.HasMany(e => e.ScientificReferences)
            .WithOne(r => r.ScientificReferencesSection)
            .HasForeignKey(r => r.ScientificReferencesSectionId);

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
