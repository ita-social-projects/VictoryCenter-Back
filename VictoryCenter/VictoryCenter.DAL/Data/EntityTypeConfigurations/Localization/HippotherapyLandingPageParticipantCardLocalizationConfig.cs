using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageParticipantCardLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageParticipantCardLocalization, HippotherapyLandingPageParticipantCard>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageParticipantCardLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
