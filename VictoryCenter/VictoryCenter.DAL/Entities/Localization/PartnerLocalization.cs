using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class PartnerLocalization : LocalizationBase<Partner>
{
    [Audited]
    public string Description { get; set; } = null!;
}
