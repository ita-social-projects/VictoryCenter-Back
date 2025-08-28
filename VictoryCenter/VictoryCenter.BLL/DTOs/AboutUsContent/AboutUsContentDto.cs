using VictoryCenter.BLL.DTOs.Images;
using ContentType = VictoryCenter.DAL.Enums.ContentType;

namespace VictoryCenter.BLL.DTOs.AboutUsContent;

public class AboutUsContentDto
{
    public ContentType ContentType { get; set; }

    public long Id { get; set; }

    public string? Description { get; init; }

    public ImageDto Image { get; set; }
}
