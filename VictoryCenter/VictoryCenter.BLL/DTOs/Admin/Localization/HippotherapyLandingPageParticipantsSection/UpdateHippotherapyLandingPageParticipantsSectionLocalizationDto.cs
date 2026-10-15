namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

public class UpdateHippotherapyLandingPageParticipantsSectionLocalizationDto
{
    public string Title { get; set; } = null!;

    public List<UpdateHippotherapyLandingPageParticipantCardLocalizationItemDto> Cards { get; set; } = [];
}
