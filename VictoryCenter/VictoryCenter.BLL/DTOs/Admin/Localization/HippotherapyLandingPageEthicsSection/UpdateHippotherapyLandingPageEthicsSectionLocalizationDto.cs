namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

public class UpdateHippotherapyLandingPageEthicsSectionLocalizationDto
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public List<UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto> Principles { get; set; } = [];
}
