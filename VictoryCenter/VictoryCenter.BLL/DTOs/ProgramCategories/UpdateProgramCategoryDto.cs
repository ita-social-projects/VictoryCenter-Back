namespace VictoryCenter.BLL.DTOs.ProgramCategories;

public record UpdateProgramCategoryDto : CreateProgramCategoryDto
{
    public long Id { get; set; }
}
