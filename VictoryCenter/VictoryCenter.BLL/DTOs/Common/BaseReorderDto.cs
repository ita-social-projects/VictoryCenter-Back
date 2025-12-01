namespace VictoryCenter.BLL.DTOs.Common;

public abstract record BaseReorderDto
{
    public List<long> OrderedIds { get; init; } = [];
}
