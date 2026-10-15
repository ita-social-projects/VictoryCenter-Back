using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageScientificReferencesSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageScientificReferencesSectionLocalization, HippotherapyLandingPageScientificReferencesSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageScientificReferencesSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
