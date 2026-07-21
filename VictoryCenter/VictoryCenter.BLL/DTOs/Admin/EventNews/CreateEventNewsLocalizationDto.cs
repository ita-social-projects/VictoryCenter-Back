namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public record CreateEventNewsLocalizationDto
{
    public long LanguageId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
}
