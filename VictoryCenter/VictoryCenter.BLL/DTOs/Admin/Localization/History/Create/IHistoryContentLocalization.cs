namespace VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;

public interface IHistoryContentLocalization
{
    long EntityId { get; }
    string? Title { get; }
    string? Description { get; }
}
