using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.AboutUsContent;

public class AboutUsSectionDto
{
    public int Id { get; set; }

    public SectionType SectionType { get; set; }

    public required string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public required List<AboutUsContentDto> Content { get; set; }
}
