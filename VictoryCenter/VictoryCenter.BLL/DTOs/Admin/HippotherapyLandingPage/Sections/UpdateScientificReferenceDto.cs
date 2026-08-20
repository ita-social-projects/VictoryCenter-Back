namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateScientificReferenceDto
{
    public long? Id { get; init; }

    public string Name { get; init; } = null!;

    public string Url { get; init; } = null!;
}
