using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record EthicsSectionDto
{
    public long Id { get; init; }

    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public IReadOnlyList<EthicsPrincipleDto> Principles { get; init; } = [];

    public ImageDto? Image { get; init; }
}
