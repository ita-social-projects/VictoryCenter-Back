using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

public record HippotherapyProgramSectionDto
{
    public long Id { get; init; }
    public long ProgramId { get; init; }
    public ProgramSectionTemplate Template { get; init; }
    public int Order { get; init; }
    public List<HippotherapyProgramSectionContentDto> Contents { get; init; } = [];
}
