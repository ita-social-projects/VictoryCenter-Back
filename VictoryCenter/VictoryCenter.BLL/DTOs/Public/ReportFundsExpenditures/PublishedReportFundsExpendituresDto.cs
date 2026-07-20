namespace VictoryCenter.BLL.DTOs.Public.ReportFundsExpenditures;

public record PublishedReportFundsExpendituresDto
{
    public PublishedReportSettingsDto Settings { get; init; } = null!;

    public PublishedFundsExpendituresGroupDto Funding { get; init; } = null!;

    public PublishedFundsExpendituresGroupDto Expenses { get; init; } = null!;

    public PublishedProgramExpendituresGroupDto Programs { get; init; } = null!;
}

public record PublishedReportSettingsDto
{
    public string DisclaimerTitle { get; init; } = "";

    public decimal ExchangeRate { get; init; }

    public int ProgramExpendituresReportingYear { get; init; }

    public DateTimeOffset PublishedAt { get; init; }
}

public record PublishedFundsExpendituresGroupDto
{
    public decimal TotalUah { get; init; }

    public decimal TotalUsd { get; init; }

    public IReadOnlyList<PublishedFundsExpendituresItemDto> Items { get; init; } = [];
}

public record PublishedFundsExpendituresItemDto
{
    public string Label { get; init; } = "";

    public decimal AmountUah { get; init; }

    public decimal AmountUsd { get; init; }
}

public record PublishedProgramExpendituresGroupDto
{
    public decimal TotalUah { get; init; }

    public decimal TotalUsd { get; init; }

    public IReadOnlyList<PublishedProgramExpendituresItemDto> Items { get; init; } = [];
}

public record PublishedProgramExpendituresItemDto
{
    public string Label { get; init; } = "";

    public int ReportingYear { get; init; }

    public decimal AmountUah { get; init; }

    public decimal AmountUsd { get; init; }
}
