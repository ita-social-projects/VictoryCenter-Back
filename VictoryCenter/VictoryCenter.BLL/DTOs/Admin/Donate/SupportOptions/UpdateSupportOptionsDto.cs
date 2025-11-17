namespace VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

public record UpdateSupportOptionsDto : ISupportOptions
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}
