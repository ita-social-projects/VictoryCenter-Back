using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class MainPartnersLocalization : LocalizationBase<MainPartners>
{
    [RichText]
    public string? Title { get; set; }
    [RichText]
    public string? Description { get; set; }
}
