namespace VictoryCenter.BLL.DTOs.Admin.MainAboutUs;

public abstract record BaseMainAboutUsDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}
