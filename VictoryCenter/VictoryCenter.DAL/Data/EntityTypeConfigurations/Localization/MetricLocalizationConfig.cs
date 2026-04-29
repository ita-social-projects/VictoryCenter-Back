using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class MetricLocalizationConfig : EntityLocalizationConfig<MetricLocalization, Metric>
{
    public override void Configure(EntityTypeBuilder<MetricLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Value);

        entity.Property(e => e.Name);
    }
}
