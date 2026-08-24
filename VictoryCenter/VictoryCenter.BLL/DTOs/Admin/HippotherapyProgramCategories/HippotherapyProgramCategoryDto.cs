using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;

public record HippotherapyProgramCategoryDto
{
    public long Id { get; init; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public List<HippotherapyProgramDto> Programs { get; init; } = [];
    public List<HippotherapyProgramCategoryLocalizationDto> Localizations { get; init; } = [];
}
