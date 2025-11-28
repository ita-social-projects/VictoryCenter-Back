namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public record SearchHippotherapyProgramDto
{
    public string SearchQuery { get; init; } = null!;

    public int? Offset { get; init; }

    public int? Limit { get; init; }
}
