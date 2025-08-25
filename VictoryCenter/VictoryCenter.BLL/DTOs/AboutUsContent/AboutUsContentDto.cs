using ContentType = VictoryCenter.DAL.Enums.ContentType;

namespace VictoryCenter.BLL.DTOs.AboutUsContent;

public class AboutUsContentDto
{
    public ContentType ContentType { get; set; }

    public long Id { get; set; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public long ImageId { get; set; }
}
