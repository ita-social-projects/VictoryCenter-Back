using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageAnalysisSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageAnalysisSectionLocalization, HippotherapyLandingPageAnalysisSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageAnalysisSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
