namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateScientificReferencesSectionDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public List<UpdateScientificReferenceDto> ScientificReferences { get; init; } = [];
}
