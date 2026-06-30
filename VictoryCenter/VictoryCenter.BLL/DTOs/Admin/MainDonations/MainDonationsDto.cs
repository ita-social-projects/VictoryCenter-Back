using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.MainDonations;

public record MainDonationsDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto? Image { get; init; }
    public ICollection<MainDonationsLocalizationDto> Localizations { get; init; } = [];
}
