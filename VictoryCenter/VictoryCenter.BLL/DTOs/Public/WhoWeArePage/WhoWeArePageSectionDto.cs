using VictoryCenter.BLL.DTOs.Common.WhoWeAreContent;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Public.WhoWeArePage;

public class WhoWeArePageSectionDto
{
    public SectionType SectionType { get; set; }

    public List<WhoWeAreContentDto> Contents { get; set; }
}
