using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageEthicsPrincipleLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageEthicsPrincipleLocalization, HippotherapyLandingPageEthicsPrinciple>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageEthicsPrincipleLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Text)
            .IsRequired();
    }
}
