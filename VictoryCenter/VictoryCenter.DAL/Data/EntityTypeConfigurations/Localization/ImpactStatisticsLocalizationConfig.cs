using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class ImpactStatisticsLocalizationConfig
    : EntityLocalizationConfig<ImpactStatisticsLocalization, ImpactStatistics>
{
    public override void Configure(EntityTypeBuilder<ImpactStatisticsLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired(false);
    }
}
