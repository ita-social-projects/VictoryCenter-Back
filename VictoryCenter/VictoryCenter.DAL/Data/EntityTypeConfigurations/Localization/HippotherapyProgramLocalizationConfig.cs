using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyProgramLocalizationConfig : EntityLocalizationConfig<HippotherapyProgramLocalization, HippotherapyProgram>
{
    public override void Configure(EntityTypeBuilder<HippotherapyProgramLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Name)
            .IsRequired();

        entity.Property(e => e.Description);

        entity.Property(e => e.Location);

        entity.Property(e => e.ParticipantsCount);

        entity.Property(e => e.MeetingsCount);
    }
}
