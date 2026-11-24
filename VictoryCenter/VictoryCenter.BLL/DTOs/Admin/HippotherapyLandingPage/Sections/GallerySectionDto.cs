namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record GallerySectionDto
{
    public long Id { get; init; }

    public string Title { get; init; } = null!;

    public IReadOnlyList<GalleryCardDto> Cards { get; init; } = [];
}
