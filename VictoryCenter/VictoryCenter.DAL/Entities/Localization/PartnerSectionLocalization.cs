using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class PartnerSectionLocalization : LocalizationBase<PartnerSection>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public string Description { get; set; } = null!;
}
