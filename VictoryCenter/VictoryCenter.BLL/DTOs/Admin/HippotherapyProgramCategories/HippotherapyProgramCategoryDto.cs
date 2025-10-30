using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;

public record HippotherapyProgramCategoryDto
{
    public long Id { get; init; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public List<HippotherapyProgramDto> Programs { get; init; } = [];
}
