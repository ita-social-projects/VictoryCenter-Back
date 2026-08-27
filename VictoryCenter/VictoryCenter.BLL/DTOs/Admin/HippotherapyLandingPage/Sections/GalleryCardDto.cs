using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;

public record GalleryCardDto
{
    public string Description { get; init; } = null!;

    public ImageDto? Image { get; init; }
}
