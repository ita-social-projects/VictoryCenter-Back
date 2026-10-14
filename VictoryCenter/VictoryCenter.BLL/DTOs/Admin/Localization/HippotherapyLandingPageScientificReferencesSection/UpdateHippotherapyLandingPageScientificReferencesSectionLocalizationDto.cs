namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;

public class UpdateHippotherapyLandingPageScientificReferencesSectionLocalizationDto
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public List<UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto> References { get; set; } = [];
}
