using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class WhoWeAreContentLocalizationConfig : EntityLocalizationConfig<WhoWeAreContentLocalization, WhoWeAreContent>
{
    public override void Configure(EntityTypeBuilder<WhoWeAreContentLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Title);

        entity.Property(e => e.Description);
    }
}
