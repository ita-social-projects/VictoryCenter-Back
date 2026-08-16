namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record ScientificReferencesSectionDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public IReadOnlyList<ScientificReferenceDto> ScientificReferences { get; init; } = [];
}
