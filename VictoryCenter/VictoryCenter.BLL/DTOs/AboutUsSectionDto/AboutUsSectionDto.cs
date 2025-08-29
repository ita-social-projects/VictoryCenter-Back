using VictoryCenter.BLL.DTOs.AboutUsContent;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.AboutUsSectionDto;

public class AboutUsSectionDto
{
    public int Id { get; set; }

    public SectionType SectionType { get; set; }

    public required string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<AboutUsContentDto> Contents { get; set; }
}
