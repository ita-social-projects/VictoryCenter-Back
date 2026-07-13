namespace VictoryCenter.DAL.Entities.Localization;

public class EventNewsLocalization : LocalizationBase<EventNews>
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}
