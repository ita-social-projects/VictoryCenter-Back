using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class EventNewsCategoryLocalization : LocalizationBase<EventNewsCategory>
{
    [Audited]
    public string Name { get; set; } = null!;
}
