using VictoryCenter.BLL.DTOs.Admin.Programs;
namespace VictoryCenter.BLL.DTOs.Admin.ProgramCategories;

public record ProgramCategoryDto
{
    public long Id { get; init; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public List<ProgramDto> Programs { get; init; } = [];
}
