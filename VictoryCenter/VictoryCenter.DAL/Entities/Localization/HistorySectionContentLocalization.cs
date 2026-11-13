using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HistoryContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HistorySectionContentLocalization : LocalizationBase<HistorySectionContent>
{
    [Audited]
    public string? Title { get; set; }

    [Audited]
    public string? Description { get; set; }
}
