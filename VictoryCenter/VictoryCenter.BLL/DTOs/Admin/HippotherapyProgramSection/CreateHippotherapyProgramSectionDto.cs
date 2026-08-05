using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

public record CreateHippotherapyProgramSectionDto
{
    public long? Id { get; set; }

    public ProgramSectionTemplate Template { get; set; }

    public int Order { get; set; }

    public List<CreateProgramSectionContentDto>? Contents { get; set; } = [];
}
