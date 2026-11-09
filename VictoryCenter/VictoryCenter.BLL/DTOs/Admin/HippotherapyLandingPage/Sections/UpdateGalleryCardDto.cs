namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateGalleryCardDto
{
    public long? Id { get; init; }
    public string Description { get; init; } = null!;

    public long? ImageId { get; init; }
}
