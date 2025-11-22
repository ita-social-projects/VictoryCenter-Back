namespace VictoryCenter.BLL.DTOs.Common;
public record LocalizationInfoDto
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;
}
