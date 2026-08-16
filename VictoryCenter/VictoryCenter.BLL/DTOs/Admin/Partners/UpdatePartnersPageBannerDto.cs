namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record UpdatePartnersPageBannerDto
{
    private readonly string? _description;
    public string Title { get; init; } = null!;
    public string Description
    {
        get => _description ?? string.Empty;
        init => _description = value?.Trim();
    }

    public long? ImageId { get; init; }

}
