namespace VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;

public class WhoWeAreSectionInfoDto
{
    public long Id { get; set; }
    public string SectionType { get; set; } = null!;

    public required string Title { get; set; }
}
