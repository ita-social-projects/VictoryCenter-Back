using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class EventNewsCategoryLocalizationConfig
    : EntityLocalizationConfig<EventNewsCategoryLocalization, EventNewsCategory>
{
    public override void Configure(EntityTypeBuilder<EventNewsCategoryLocalization> builder)
    {
        base.Configure(builder);

        builder.Property(localization => localization.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(localization => new { localization.LanguageId, localization.Name })
            .IsUnique();
    }
}
