using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HistorySectionContentLocalizationConfig : EntityLocalizationConfig<HistorySectionContentLocalization, HistorySectionContent>
{
    public override void Configure(EntityTypeBuilder<HistorySectionContentLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .HasMaxLength(60)
            .IsRequired(false);

        entity.Property(e => e.Description)
            .HasMaxLength(600)
            .IsRequired(false);
    }
}
