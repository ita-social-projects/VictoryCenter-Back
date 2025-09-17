using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.WhoWeAreContent;

public class CreateWhoWeAreContentDto
{
    public ContentType ContentType { get; init; }

    public long Id { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public long? ImageId { get; init; }
}
