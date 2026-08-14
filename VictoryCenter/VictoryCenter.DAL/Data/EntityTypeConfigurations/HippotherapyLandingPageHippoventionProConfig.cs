using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageHippoventionProConfig : IEntityTypeConfiguration<HippotherapyLandingPageHippoventionPro>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageHippoventionPro> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.HippoventionCenterSectionId)
            .IsRequired();

        entity.Property(e => e.Text)
            .IsRequired();

        entity.Property(e => e.Priority)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasIndex(e => new { e.HippoventionCenterSectionId, e.Priority })
            .IsUnique();
    }
}
