namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateIntroSectionDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public long? ImageId { get; init; }
}
