namespace VictoryCenter.BLL.DTOs.Donations.Common;

public class DonationsGroupedDto
{
    public required CurrencyDonationsDto Uah { get; set; }
    public required CurrencyDonationsDto Usd { get; set; }
    public required CurrencyDonationsDto Eur { get; set; }
}
