using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;

public record ReportFundsExpendituresCategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public ReportFundsExpendituresType Type { get; init; }
}
