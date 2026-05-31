namespace VictoryCenter.BLL.DTOs.Common;

public class EmailDto
{
    public required string From { get; init; }

    public required IEnumerable<string> To { get; init; }

    public required string Subject { get; init; }

    public string? TextBody { get; init; }

    public string? HtmlBody { get; init; }

    public IEnumerable<string>? ReplyTo { get; init; }
}
