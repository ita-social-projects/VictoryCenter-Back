using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;

public class CardContentDto : WhoWeAreContentDto
{
    public string? Description { get; init; }

    public ImageDto Image { get; set; }
}
