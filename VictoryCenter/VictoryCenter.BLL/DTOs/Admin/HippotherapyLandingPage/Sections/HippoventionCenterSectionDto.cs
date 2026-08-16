using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record HippoventionCenterSectionDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public IReadOnlyList<string> Pros { get; init; } = [];

    public ImageDto? Image { get; init; }
}
