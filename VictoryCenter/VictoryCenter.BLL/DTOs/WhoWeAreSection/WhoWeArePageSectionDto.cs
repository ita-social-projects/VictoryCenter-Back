using VictoryCenter.BLL.DTOs.WhoWeAreContent;

namespace VictoryCenter.BLL.DTOs.WhoWeAreSection;

public class WhoWeArePageSectionDto
{
    public string SectionType { get; set; } = null!;

    public List<WhoWeAreContentDto> Contents { get; set; }
}
