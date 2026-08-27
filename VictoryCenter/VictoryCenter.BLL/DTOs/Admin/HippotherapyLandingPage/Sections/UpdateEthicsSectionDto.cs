namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateEthicsSectionDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public List<string> Principles { get; init; } = [];

    public long? ImageId { get; init; }
}
