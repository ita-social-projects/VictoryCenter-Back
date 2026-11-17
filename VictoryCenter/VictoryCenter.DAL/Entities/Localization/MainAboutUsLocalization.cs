using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class MainAboutUsLocalization : LocalizationBase<MainAboutUs>
{
    [Audited]
    [RichText]
    public string? Title { get; set; }

    [Audited]
    [RichText]
    public string? Description { get; set; }
}
