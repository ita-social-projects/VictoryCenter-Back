using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class EventsIntroSectionLocalizationConfig
    : EntityLocalizationConfig<EventsIntroSectionLocalization, EventsIntroSection>
{
    public override void Configure(EntityTypeBuilder<EventsIntroSectionLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.EventsBlockTitle)
            .IsRequired();

        entity.Property(e => e.PageDescription)
            .IsRequired();
    }
}
