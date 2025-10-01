namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnerImageDto : CreatePartnerImageDto
{
    public long? ImageId { get; init; } = null;
}
