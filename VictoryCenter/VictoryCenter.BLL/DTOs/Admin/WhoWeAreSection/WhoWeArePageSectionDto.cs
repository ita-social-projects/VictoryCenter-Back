using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;

namespace VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

public class WhoWeArePageSectionDto
{
    public string SectionType { get; set; } = null!;

    public List<WhoWeAreContentDto> Contents { get; set; }
}
