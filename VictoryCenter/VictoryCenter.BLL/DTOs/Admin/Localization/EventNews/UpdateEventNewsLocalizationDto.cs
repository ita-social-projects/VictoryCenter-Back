namespace VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;

public record UpdateEventNewsLocalizationDto
{
    public string Title { get; init; } = null!;

    public string? Description { get; init; }

    public string? AdditionalDescription { get; init; }
}
