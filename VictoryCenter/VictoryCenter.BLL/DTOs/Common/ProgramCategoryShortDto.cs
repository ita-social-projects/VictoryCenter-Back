using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.DTOs.Common;

public record ProgramCategoryShortDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public List<HippotherapyProgramCategoryLocalizationDto> Localizations { get; init; } = [];
}
