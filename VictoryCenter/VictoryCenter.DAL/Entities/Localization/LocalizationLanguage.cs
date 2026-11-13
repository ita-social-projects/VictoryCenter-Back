using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

[Auditable]
public class LocalizationLanguage : BaseEntity
{
    // ISO 639-1 is used for distinguishing languages
    [Audited]
    public string Code { get; set; } = null!;

    [Audited]
    public string Name { get; set; } = null!;
}
