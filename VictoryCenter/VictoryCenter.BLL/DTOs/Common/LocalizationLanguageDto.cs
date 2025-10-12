namespace VictoryCenter.BLL.DTOs.Common;

public record LocalizationLanguageDto
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;
}
