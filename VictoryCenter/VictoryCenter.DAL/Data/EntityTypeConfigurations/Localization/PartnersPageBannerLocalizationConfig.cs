using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class PartnersPageBannerLocalizationConfig : EntityLocalizationConfig<PartnersPageBannerLocalization, PartnersPageBanner>
{
    public override void Configure(EntityTypeBuilder<PartnersPageBannerLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title)
            .IsRequired();

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
