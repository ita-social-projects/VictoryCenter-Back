namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record GallerySectionDto
{
    public string Title { get; init; } = null!;

    public IReadOnlyList<GalleryCardDto> Cards { get; init; } = [];
}
