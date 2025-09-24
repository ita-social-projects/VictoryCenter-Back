using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

public class WhoWeArePageSectionDto
{
    public SectionType SectionType { get; set; }

    public List<WhoWeAreContentDto> Contents { get; set; }
}
