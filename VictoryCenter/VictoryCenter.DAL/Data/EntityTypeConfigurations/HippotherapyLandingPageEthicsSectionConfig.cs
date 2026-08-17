using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageEthicsSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageEthicsSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageEthicsSection> entity)
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
            .HasForeignKey<HippotherapyLandingPageEthicsSection>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasMany(e => e.EthicsPrinciples)
            .WithOne(p => p.EthicsSection)
            .HasForeignKey(p => p.EthicsSectionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
