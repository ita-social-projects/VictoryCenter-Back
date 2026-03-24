namespace VictoryCenter.BLL.DTOs.Admin.SocialLinks;

public record UpdateSocialLinkDto : BaseSocialLinkDto
{
    public long Id { get; init; }
}
