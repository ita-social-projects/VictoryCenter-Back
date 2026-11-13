using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class EventNewsLocalization : LocalizationBase<EventNews>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public string? Description { get; set; }

    [Audited]
    public string? AdditionalDescription { get; set; }
}
