using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Data.EntityTypeConfigurations.Localization;

public class ProgramSectionContentLocalizationConfig
    : EntityLocalizationConfig<ProgramSectionContentLocalization, ProgramSectionContent>
{
    public override void Configure(EntityTypeBuilder<ProgramSectionContentLocalization> entity)
    {
        base.Configure(entity);
    }
}
