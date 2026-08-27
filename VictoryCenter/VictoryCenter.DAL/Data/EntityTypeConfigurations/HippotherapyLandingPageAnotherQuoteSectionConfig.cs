using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageAnotherQuoteSectionConfig : IEntityTypeConfiguration<HippotherapyLandingPageAnotherQuoteSection>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPageAnotherQuoteSection> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.HippotherapyLandingPageId)
            .IsRequired();

        entity.Property(e => e.QuoteText)
            .IsRequired();

        entity.Property(e => e.AuthorName);

        entity.Property(e => e.ImageId);

        entity.HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<HippotherapyLandingPageAnotherQuoteSection>(e => e.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
