using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.ProgramCategories;

namespace VictoryCenter.BLL.DTOs.Programs;

public class PublishedProgramDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ImageDTO? Image { get; set; }
    public List<ProgramCategoryShortDto> Categories { get; set; } = new List<ProgramCategoryShortDto>();
}
