namespace VictoryCenter.BLL.DTOs.Admin.MainPages;

public abstract record BaseMainPageDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public long? ImageId { get; init; }
}
