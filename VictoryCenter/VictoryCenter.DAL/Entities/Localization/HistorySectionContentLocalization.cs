using VictoryCenter.DAL.Entities.HistoryContents;

namespace VictoryCenter.DAL.Entities.Localization;

public class HistorySectionContentLocalization : LocalizationBase<HistorySectionContent>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}
