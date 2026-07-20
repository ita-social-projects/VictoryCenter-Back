using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class PublishedReportFundsExpendituresSnapshot : BaseEntity
{
    public string DisclaimerTitle { get; set; } = "";

    public string? DisclaimerTitleEn { get; set; }

    public decimal ExchangeRate { get; set; }

    public int ProgramExpendituresReportingYear { get; set; }

    public DateTimeOffset PublishedAt { get; set; }
}
