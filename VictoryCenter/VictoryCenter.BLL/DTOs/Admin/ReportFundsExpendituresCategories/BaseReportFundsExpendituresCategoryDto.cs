using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;

public abstract record BaseReportFundsExpendituresCategoryDto
{
    public string Name { get; init; } = null!;
    public ReportFundsExpendituresType Type { get; init; }
}
