namespace VictoryCenter.BLL.DTOs.Common;

public record LocalizationLanguageDto : LocalizationInfoDto
{
    public string Name { get; set; } = null!;
}
