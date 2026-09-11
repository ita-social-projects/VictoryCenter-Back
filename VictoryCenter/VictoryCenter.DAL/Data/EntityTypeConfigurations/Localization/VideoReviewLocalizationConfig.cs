using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class VideoReviewLocalizationConfig
    : EntityLocalizationConfig<VideoReviewLocalization, VideoReview>
{
    public override void Configure(EntityTypeBuilder<VideoReviewLocalization> builder)
    {
        base.Configure(builder);

        builder.Property(localization => localization.Title)
            .IsRequired()
            .HasMaxLength(200);
    }
}
