using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;

public class CreateReportFundsExpendituresCategoryLocalizationDto
    : UpdateReportFundsExpendituresCategoryLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
}
