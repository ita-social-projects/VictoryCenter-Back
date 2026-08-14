namespace VictoryCenter.BLL.DTOs.Public.ReportFundsExpenditures;

public record PublishedMediaBlockDto
{
    public string Title { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int? Value { get; init; }
}

public record PublishedReportMediaSettingsDto
{
    public PublishedMediaBlockDto CollectedFunds { get; init; } = new();
    public PublishedMediaBlockDto ChangedLives { get; init; } = new();
}
