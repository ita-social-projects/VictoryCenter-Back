using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class PartnersPageBannerLocalization : LocalizationBase<PartnersPageBanner>
{
    [Audited]
    [RichText]
    public string Title { get; set; } = null!;

    [Audited]
    public string Description { get; set; } = null!;
}
