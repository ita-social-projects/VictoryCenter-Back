using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.Base;

public interface ITranslationStatusFilterDto
{
    TranslationStatus? TranslationStatus { get; set; }
}
