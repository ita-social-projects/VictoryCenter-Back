using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class FeedbackReviewLocalizationConfig
    : EntityLocalizationConfig<FeedbackReviewLocalization, FeedbackReview>
{
    public override void Configure(EntityTypeBuilder<FeedbackReviewLocalization> builder)
    {
        base.Configure(builder);

        builder.Property(localization => localization.AuthorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(localization => localization.Text)
            .IsRequired()
            .HasMaxLength(500);
    }
}
