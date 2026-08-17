using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageScientificReferenceConfig : IEntityTypeConfiguration<HippotherapyLandingPageScientificReference>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageScientificReference> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.ScientificReferencesSectionId)
            .IsRequired();

        entity.Property(e => e.Name)
            .IsRequired();

        entity.Property(e => e.Url)
            .IsRequired();

        entity.Property(e => e.Priority)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => new { e.ScientificReferencesSectionId, e.Priority })
            .IsUnique();
    }
}
