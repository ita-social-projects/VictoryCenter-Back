using VictoryCenter.BLL.DTOs.Admin.Common;
namespace VictoryCenter.BLL.DTOs.Admin.Programs;

public record ProgramsFilterDto : BaseFilterDto
{
    public List<long>? CategoryId { get; init; }
}
