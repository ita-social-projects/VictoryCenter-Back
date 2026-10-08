using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageAnotherQuoteSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageAnotherQuoteSectionLocalization, HippotherapyLandingPageAnotherQuoteSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageAnotherQuoteSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.QuoteText)
            .IsRequired();

        entity.Property(e => e.AuthorName);
    }
}
