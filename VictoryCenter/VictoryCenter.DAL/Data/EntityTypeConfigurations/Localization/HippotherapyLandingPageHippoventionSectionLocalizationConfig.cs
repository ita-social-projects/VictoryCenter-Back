using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageHippoventionSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageHippoventionSectionLocalization, HippotherapyLandingPageHippoventionSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageHippoventionSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
