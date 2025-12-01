namespace VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

public abstract record BaseSupportOptionsDto
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}
