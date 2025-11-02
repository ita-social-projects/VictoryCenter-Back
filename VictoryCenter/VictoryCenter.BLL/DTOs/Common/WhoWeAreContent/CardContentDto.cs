namespace VictoryCenter.BLL.DTOs.Common.WhoWeAreContent;

public class CardContentDto : WhoWeAreContentDto
{
    public string? Description { get; init; }

    public ImageDto? Image { get; set; }
}
