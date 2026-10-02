using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageIntroSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageIntroSectionLocalization, HippotherapyLandingPageIntroSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageIntroSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
