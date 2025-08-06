using VictoryCenter.DAL.Enums;
namespace VictoryCenter.BLL.DTOs.Programs;

public class ProgramFilterRequestDto
{
    public int? Offset { get; init; }

    public int? Limit { get; init; }

    public Status? Status { get; init; }

    public List<long>? CategoryId { get; init; }
}
