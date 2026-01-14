using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

public record CreateHippotherapyProgramSectionDto
{
    public ProgramSectionTemplate Template { get; set; }

    public int Order { get; set; }

    public List<CreateProgramSectionContentDto> Contents { get; set; } = [];
}
