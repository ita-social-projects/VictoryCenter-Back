namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record UpdateGallerySectionDto
{
    public string Title { get; init; } = null!;

    public List<UpdateGalleryCardDto> Cards { get; init; } = [];
}
