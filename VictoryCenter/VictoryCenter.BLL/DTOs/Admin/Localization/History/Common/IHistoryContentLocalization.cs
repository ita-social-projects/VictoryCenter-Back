namespace VictoryCenter.BLL.DTOs.Admin.Localization.History.Common;

public interface IHistoryContentLocalization
{
    long EntityId { get; }
    string? Title { get; }
    string? Description { get; }
}
