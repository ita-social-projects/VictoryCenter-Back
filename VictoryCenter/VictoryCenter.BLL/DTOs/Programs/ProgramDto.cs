using VictoryCenter.BLL.DTOs.Images;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Programs;

public class ProgramDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public Status Status { get; set; }

    public ImageDTO? Image { get; set; }
    public List<ProgramCategoryShortDto> Categories { get; set; } = new List<ProgramCategoryShortDto>();
}
