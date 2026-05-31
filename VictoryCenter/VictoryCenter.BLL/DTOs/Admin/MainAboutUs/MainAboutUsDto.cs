using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.DTOs.Admin.MainAboutUs;

public record MainAboutUsDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ICollection<MainAboutUsLocalizationDto> Localizations { get; init; } = [];
}
