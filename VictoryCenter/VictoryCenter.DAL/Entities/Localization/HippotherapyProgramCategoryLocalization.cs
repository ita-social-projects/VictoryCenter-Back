using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class HippotherapyProgramCategoryLocalization : LocalizationBase<HippotherapyProgramCategory>
{
    [Audited]
    public string Name { get; set; } = null!;
}
