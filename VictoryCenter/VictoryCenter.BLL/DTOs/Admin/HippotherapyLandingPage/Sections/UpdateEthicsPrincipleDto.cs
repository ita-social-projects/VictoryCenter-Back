namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
public record UpdateEthicsPrincipleDto
{
    public long? Id { get; init; }
    public string Text { get; init; } = null!;
}
