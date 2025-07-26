namespace VictoryCenter.BLL.DTOs.Payment.WayForPay;

public class WayForPayPurchaseRequest
{
    required public string MerchantAccount { get; init; }
    required public string MerchantDomainName { get; init; }
    required public string MerchantSignature { get; init; }
    required public string OrderReference { get; init; }
    required public long OrderDate { get; init; }
    required public decimal Amount { get; init; }
    required public Currency Currency { get; init; }
    required public string[] ProductName { get; init; }
    required public decimal[] ProductPrice { get; init; }
    required public int[] ProductCount { get; init; }
    public string? ReturnUrl { get; set; }
    public string? ServiceUrl { get; set; }
    public string? RegularBehavior { get; set; }
    public string? RegularMode { get; set; }
    public decimal? RegularAmount { get; set; }
    public bool? RegularOn { get; set; }
}
