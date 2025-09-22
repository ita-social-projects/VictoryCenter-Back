using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

public class WhoWeAreSectionDto
{
    public SectionType SectionType { get; set; }

    public required string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<WhoWeAreContentDto> Contents { get; set; }
}
