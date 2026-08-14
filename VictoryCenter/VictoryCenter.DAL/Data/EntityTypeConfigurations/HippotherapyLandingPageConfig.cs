using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations;

internal class HippotherapyLandingPageConfig : IEntityTypeConfiguration<HippotherapyLandingPage>
{
    public void Configure(EntityTypeBuilder<HippotherapyLandingPage> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.HasOne(e => e.IntroSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageIntroSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.DescriptionSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageDescriptionSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.QuoteSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageQuoteSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.HippoventionSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageHippoventionSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.HippoventionCenterSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageHippoventionCenterSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.AdvantagesSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageAdvantagesSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.AnalysisSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageAnalysisSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.ScientificReferencesSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageScientificReferencesSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.AnotherQuoteSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageAnotherQuoteSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.ParticipantsSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageParticipantsSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.EthicsSection)
            .WithOne(s => s.HippotherapyLandingPage)
            .HasForeignKey<HippotherapyLandingPageEthicsSection>(s => s.HippotherapyLandingPageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
