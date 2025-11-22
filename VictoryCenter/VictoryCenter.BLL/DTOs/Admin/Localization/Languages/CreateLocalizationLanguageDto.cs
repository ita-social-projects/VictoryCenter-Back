namespace VictoryCenter.BLL.DTOs.Admin.Localization.Languages;

public record CreateLocalizationLanguageDto
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;
}
