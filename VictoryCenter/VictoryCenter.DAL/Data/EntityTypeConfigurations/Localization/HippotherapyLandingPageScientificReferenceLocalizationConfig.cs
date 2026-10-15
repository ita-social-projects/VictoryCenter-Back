using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageScientificReferenceLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageScientificReferenceLocalization, HippotherapyLandingPageScientificReference>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageScientificReferenceLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Name)
            .IsRequired();
    }
}
