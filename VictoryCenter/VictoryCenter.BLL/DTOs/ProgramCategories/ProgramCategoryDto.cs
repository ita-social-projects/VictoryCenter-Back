using VictoryCenter.BLL.DTOs.Programs;
namespace VictoryCenter.BLL.DTOs.ProgramCategories;

public record ProgramCategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProgramDto> Programs { get; set; } = new List<ProgramDto>();
}
