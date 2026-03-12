using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.Common;

public class TranslationStatusInfoDto
{
    public long LanguageId { get; set; }

    public TranslationStatus TranslationStatus { get; set; }
}
