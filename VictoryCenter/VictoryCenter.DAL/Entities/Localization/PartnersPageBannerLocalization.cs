using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class PartnersPageBannerLocalization : LocalizationBase<PartnersPageBanner>
{
    [RichText]
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
}
