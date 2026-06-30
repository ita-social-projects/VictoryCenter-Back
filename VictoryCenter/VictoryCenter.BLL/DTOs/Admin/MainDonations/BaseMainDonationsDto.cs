namespace VictoryCenter.BLL.DTOs.Admin.MainDonations;

public abstract record BaseMainDonationsDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public long? ImageId { get; init; }
}
