using VictoryCenter.BLL.DTOs.Admin.Common;

namespace VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;

public record HypotherapyProgramsFilterDto : BaseFilterDto
{
    public List<long>? CategoryId { get; init; }
}
