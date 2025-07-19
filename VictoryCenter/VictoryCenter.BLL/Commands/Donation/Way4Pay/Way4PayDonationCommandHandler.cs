using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Donation.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment.Donation;
using VictoryCenter.BLL.DTOs.Payment.Way4Pay;
using VictoryCenter.BLL.Options.Donation;

namespace VictoryCenter.BLL.Commands.Donation.Way4Pay;

public class Way4PayDonationCommandHandler : IDonationCommandHandler<DonationCommand, Result<DonationResponseDto>>
{
    private readonly IOptions<Way4PayOptions> _way4PayOptions;
    private readonly IHttpClientFactory _httpClientFactory;

    public Way4PayDonationCommandHandler(IOptions<Way4PayOptions> way4PayOptions, IHttpClientFactory httpClientFactory)
    {
        _way4PayOptions = way4PayOptions;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<DonationResponseDto>> Handle(DonationCommand request, CancellationToken cancellationToken)
    {
        var orderReference = Guid.CreateVersion7();
        var orderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var merchantSignature = GenerateMerchantSignature(request, orderReference, orderDate);

        var purchaseRequest = new Way4PayPurchaseRequest()
        {
            Amount = request.Request.Amount,
            Currency = request.Request.Currency,
            MerchantAccount = _way4PayOptions.Value.MerchantLogin,
            MerchantDomainName = _way4PayOptions.Value.MerchantDomainName,
            OrderDate = orderDate,
            OrderReference = orderReference.ToString(),
            ProductCount = [1],
            ProductName = [PaymentConstants.ProductName],
            ProductPrice = [request.Request.Amount],
            MerchantSignature = merchantSignature,
            ReturnUrl = request.Request.ReturnUrl
        };

        if (request.Request.IsSubscription)
        {
            purchaseRequest.RegularBehavior = "preset";
            purchaseRequest.RegularAmount = request.Request.Amount;
            purchaseRequest.RegularMode = "monthly";
            purchaseRequest.RegularOn = true;
        }

        var keyValues = new Dictionary<string, string>
        {
            ["merchantAccount"] = purchaseRequest.MerchantAccount,
            ["merchantDomainName"] = purchaseRequest.MerchantDomainName,
            ["orderReference"] = purchaseRequest.OrderReference,
            ["orderDate"] = purchaseRequest.OrderDate.ToString(),
            ["amount"] = purchaseRequest.Amount.ToString(CultureInfo.InvariantCulture),
            ["currency"] = purchaseRequest.Currency,
            ["productName[]"] = purchaseRequest.ProductName[0],
            ["productCount[]"] = purchaseRequest.ProductCount[0].ToString(CultureInfo.InvariantCulture),
            ["productPrice[]"] = purchaseRequest.ProductPrice[0].ToString(CultureInfo.InvariantCulture),
            ["merchantSignature"] = purchaseRequest.MerchantSignature,
        };

        if (purchaseRequest.RegularOn.HasValue && purchaseRequest.RegularOn.Value)
        {
            keyValues["regularOn"] = "1";
            keyValues["regularAmount"] = purchaseRequest.RegularAmount?.ToString()!;
            keyValues["regularMode"] = purchaseRequest.RegularMode!;
            keyValues["regularBehavior"] = purchaseRequest.RegularBehavior!;
            keyValues["regularCount"] = "12";
        }

        if (!string.IsNullOrWhiteSpace(purchaseRequest.ReturnUrl))
        {
            keyValues["returnUrl"] = purchaseRequest.ReturnUrl;
        }

        var content = new FormUrlEncodedContent(keyValues);

        var client = _httpClientFactory.CreateClient("Way4PayClient");

        var httpRequestMessage = new HttpRequestMessage()
        {
            RequestUri = new Uri(_way4PayOptions.Value.ApiUrl),
            Method = HttpMethod.Post,
            Content = content
        };

        var response = await client.SendAsync(httpRequestMessage, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Found or HttpStatusCode.SeeOther or HttpStatusCode.Moved)
        {
            var redirectUrl = response.Headers.Location?.ToString();
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return Result.Ok(new DonationResponseDto()
                {
                    PaymentUrl = redirectUrl
                });
            }
        }

        return Result.Fail(response.ReasonPhrase);
    }

    private string GenerateMerchantSignature(DonationCommand request, Guid orderReference, long orderDate)
    {
        var concatenatedValues = string.Join(
            ';',
            _way4PayOptions.Value.MerchantLogin,
            _way4PayOptions.Value.MerchantDomainName,
            orderReference,
            orderDate,
            request.Request.Amount,
            request.Request.Currency,
            PaymentConstants.ProductName,
            1,
            request.Request.Amount);

        var secretKeyBytes = Encoding.UTF8.GetBytes(_way4PayOptions.Value.MerchantSecretKey);
        var signatureBytes = Encoding.UTF8.GetBytes(concatenatedValues);

        using var hmac = new HMACMD5(secretKeyBytes);

        var bytes = hmac.ComputeHash(signatureBytes);
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
