using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class MainPageLocalizationConfig : EntityLocalizationConfig<MainPageLocalization, MainPage>
{
    public override void Configure(EntityTypeBuilder<MainPageLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired(false);

        entity.Property(e => e.Description)
            .IsRequired(false);
    }
}
