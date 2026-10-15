using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyLandingPageParticipantsSectionLocalizationConfig
    : EntityLocalizationConfig<HippotherapyLandingPageParticipantsSectionLocalization, HippotherapyLandingPageParticipantsSection>
{
    public override void Configure(EntityTypeBuilder<HippotherapyLandingPageParticipantsSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();
    }
}
