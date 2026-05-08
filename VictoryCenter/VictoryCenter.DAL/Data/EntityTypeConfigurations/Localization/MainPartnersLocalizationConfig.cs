using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class MainPartnersLocalizationConfig : EntityLocalizationConfig<MainPartnersLocalization, MainPartners>
{
    public override void Configure(EntityTypeBuilder<MainPartnersLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired(false);

        entity.Property(e => e.Description)
            .IsRequired(false);
    }
}
