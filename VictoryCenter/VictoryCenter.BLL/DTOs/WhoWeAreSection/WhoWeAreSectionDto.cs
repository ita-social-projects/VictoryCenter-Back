using VictoryCenter.BLL.DTOs.WhoWeAreContent;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.WhoWeAreSection;

public class WhoWeAreSectionDto
{
    public int Id { get; set; }

    public SectionType SectionType { get; set; }

    public required string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<WhoWeAreContentDto> Contents { get; set; }
}
