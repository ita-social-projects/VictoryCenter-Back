using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class MainDonationsLocalization : LocalizationBase<MainDonations>
{
    [Audited]
    [RichText]
    public string? Title { get; set; }

    [Audited]
    [RichText]
    public string? Description { get; set; }
}
