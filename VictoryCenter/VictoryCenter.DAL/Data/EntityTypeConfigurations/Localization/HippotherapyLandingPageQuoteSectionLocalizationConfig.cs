using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageQuoteSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageQuoteSectionLocalization, HippotherapyLandingPageQuoteSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageQuoteSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.QuoteText)
            .IsRequired();

        entity.Property(e => e.AuthorName);
    }
}
