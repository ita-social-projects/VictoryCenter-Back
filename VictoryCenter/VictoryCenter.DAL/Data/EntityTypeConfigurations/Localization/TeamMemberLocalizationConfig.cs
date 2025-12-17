using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class TeamMemberLocalizationConfig : EntityLocalizationConfig<TeamMemberLocalization, TeamMember>
{
    public override void Configure(EntityTypeBuilder<TeamMemberLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.FullName)
            .IsRequired();

        entity.Property(e => e.Description);
    }
}
