using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class MainPartnersLocalization : LocalizationBase<MainPartners>
{
    [Audited]
    [RichText]
    public string? Title { get; set; }

    [Audited]
    [RichText]
    public string? Description { get; set; }
}
