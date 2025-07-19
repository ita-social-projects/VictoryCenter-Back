using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options.Donation;

public class Way4PayOptions
{
    public const string Position = "PaymentSystemsConfigurations:Way4Pay";

    [Required]
    public string MerchantLogin { get; init; } = null!;

    [Required]
    public string MerchantSecretKey { get; init; } = null!;

    [Required]
    public string MerchantDomainName { get; init; } = null!;

    [Required]
    public string ApiUrl { get; init; } = null!;
}
