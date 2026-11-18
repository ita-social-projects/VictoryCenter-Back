using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class PublishedReportFundsExpendituresSnapshot : BaseEntity
{
    [Audited]
    public string DisclaimerTitle { get; set; } = "";

    [Audited]
    public string? DisclaimerTitleEn { get; set; }

    [Audited]
    public decimal ExchangeRate { get; set; }

    [Audited]
    public int ProgramExpendituresReportingYear { get; set; }

    [Audited]
    public DateTimeOffset PublishedAt { get; set; }
}
