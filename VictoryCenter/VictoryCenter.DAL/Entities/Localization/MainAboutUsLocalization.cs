using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class MainAboutUsLocalization : LocalizationBase<MainAboutUs>
{
    [RichText]
    public string? Title { get; set; }
    [RichText]
    public string? Description { get; set; }
}
