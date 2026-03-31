using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.SocialLinks;

public record BaseSocialLinkDto
{
    public SocialPlatform SocialPlatform { get; set; }
    public string Url { get; set; } = null!;
}
