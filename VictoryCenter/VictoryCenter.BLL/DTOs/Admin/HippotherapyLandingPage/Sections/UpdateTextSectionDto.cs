namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateTextSectionDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;
}
