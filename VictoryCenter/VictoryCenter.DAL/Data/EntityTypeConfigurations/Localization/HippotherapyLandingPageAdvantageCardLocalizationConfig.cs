using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageAdvantageCardLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageAdvantageCardLocalization, HippotherapyLandingPageAdvantageCard>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageAdvantageCardLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
