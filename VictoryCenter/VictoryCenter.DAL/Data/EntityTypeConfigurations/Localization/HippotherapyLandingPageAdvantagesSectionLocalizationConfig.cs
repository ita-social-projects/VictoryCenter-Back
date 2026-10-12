using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageAdvantagesSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageAdvantagesSectionLocalization, HippotherapyLandingPageAdvantagesSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageAdvantagesSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();
    }
}
