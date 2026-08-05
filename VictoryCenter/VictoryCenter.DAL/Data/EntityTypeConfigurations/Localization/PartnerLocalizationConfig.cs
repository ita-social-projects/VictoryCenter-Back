using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class PartnerLocalizationConfig : EntityLocalizationConfig<PartnerLocalization, Partner>
{
    public override void Configure(EntityTypeBuilder<PartnerLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Description)
            .IsRequired();
    }
}
