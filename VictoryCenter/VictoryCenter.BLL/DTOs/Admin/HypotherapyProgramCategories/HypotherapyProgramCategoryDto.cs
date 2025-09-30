using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;

namespace VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;

public record HypotherapyProgramCategoryDto
{
    public long Id { get; init; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public List<HypotherapyProgramDto> Programs { get; init; } = [];
}
