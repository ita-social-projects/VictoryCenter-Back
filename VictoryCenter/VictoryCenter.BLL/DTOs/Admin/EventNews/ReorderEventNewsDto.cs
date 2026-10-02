namespace VictoryCenter.BLL.DTOs.Admin.EventNews;

public class ReorderEventNewsDto
{
    public long CategoryId { get; init; }
    public List<long> Ids { get; init; } = [];
}
