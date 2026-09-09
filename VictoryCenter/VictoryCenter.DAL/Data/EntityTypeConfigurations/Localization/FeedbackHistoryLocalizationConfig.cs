using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class FeedbackHistoryLocalizationConfig
    : EntityLocalizationConfig<FeedbackHistoryLocalization, FeedbackHistory>
{
    public override void Configure(EntityTypeBuilder<FeedbackHistoryLocalization> builder)
    {
        base.Configure(builder);

        builder.Property(localization => localization.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(localization => localization.Story)
            .IsRequired()
            .HasMaxLength(1000);
    }
}
