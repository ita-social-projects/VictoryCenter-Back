using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.WhoWeAreContent;

public class CreateWhoWeAreContentDto
{
    public ContentType ContentType { get; set; }

    public long Id { get; set; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public long ImageId { get; set; }
}
