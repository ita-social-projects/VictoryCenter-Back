using VictoryCenter.BLL.DTOs.Images;

namespace VictoryCenter.BLL.DTOs.WhoWeAreContent;

public class CardContentDto : WhoWeAreContentDto
{
    public string? Description { get; init; }

    public ImageDto Image { get; set; }
}
