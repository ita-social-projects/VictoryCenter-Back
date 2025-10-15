using VictoryCenter.BLL.DTOs.Admin.Common;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public record HippotherapyProgramsFilterDto : BaseFilterDto
{
    public List<long>? CategoryId { get; init; }
}
