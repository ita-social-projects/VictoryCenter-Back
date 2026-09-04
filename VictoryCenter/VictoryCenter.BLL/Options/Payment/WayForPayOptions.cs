using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options.Payment;

public class WayForPayOptions
{
    static readonly public string Position = "PaymentSystemsConfigurations:Way4Pay";

    [Required]
    public string MerchantLogin { get; init; } = null!;

    [Required]
    public string MerchantSecretKey { get; init; } = null!;

    [Required]
    public string MerchantDomainName { get; init; } = null!;

    [Required]
    public string ApiUrl { get; init; } = null!;

    [MinLength(1)]
    public string[] AllowedReturnUrlHosts { get; init; } = [];
}
