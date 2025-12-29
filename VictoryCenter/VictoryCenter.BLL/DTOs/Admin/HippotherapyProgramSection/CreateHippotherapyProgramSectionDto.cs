using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

public record CreateHippotherapyProgramSectionDto
{
    public ProgramSectionTemplate Template { get; set; }
    public int Order { get; set; }
    public List<string> Titles { get; set; } = [];
    public List<string> Descriptions { get; set; } = [];
    public List<long> ImageIds { get; set; } = [];
}
