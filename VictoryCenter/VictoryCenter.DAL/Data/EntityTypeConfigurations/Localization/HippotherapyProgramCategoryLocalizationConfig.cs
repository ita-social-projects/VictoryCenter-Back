using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class HippotherapyProgramCategoryLocalizationConfig: EntityLocalizationConfig<HippotherapyProgramCategoryLocalization, HippotherapyProgramCategory>
{
    public override void Configure(EntityTypeBuilder<HippotherapyProgramCategoryLocalization> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Name)
            .IsRequired();
    }
}
