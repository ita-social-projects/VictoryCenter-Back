using VictoryCenter.BLL.DTOs.Admin.Localization.Common;

namespace VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

public class WhoWeAreSectionInfoDto
{
    public long Id { get; set; }
    public string SectionType { get; set; } = null!;
    public List<TranslationStatusInfoDto> TranslationStatuses { get; set; } = [];

    public required string Title { get; set; }
}
