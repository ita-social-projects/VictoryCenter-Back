using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.Localization;

public class LocalizationLanguage : BaseEntity
{
    // ISO 639-1 is used for distinguishing languages
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
