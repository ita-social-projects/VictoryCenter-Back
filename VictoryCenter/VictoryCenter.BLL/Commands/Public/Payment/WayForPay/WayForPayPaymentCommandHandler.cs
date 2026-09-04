using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Public.Payment.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.DTOs.Public.Payment.WayForPay;
using VictoryCenter.BLL.Options.Payment;

namespace VictoryCenter.BLL.Commands.Public.Payment.WayForPay;

public class WayForPayPaymentCommandHandler : IPaymentCommandHandler<PaymentCommand, Result<PaymentResponseDto>>
{
    private readonly IOptions<WayForPayOptions> _way4PayOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WayForPayPaymentCommandHandler> _logger;

    public WayForPayPaymentCommandHandler(IOptions<WayForPayOptions> way4PayOptions, IHttpClientFactory httpClientFactory, ILogger<WayForPayPaymentCommandHandler> logger)
    {
        _way4PayOptions = way4PayOptions;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result<PaymentResponseDto>> Handle(PaymentCommand request, CancellationToken cancellationToken)
    {
        var orderReference = Guid.CreateVersion7();
        var orderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var merchantSignature = GenerateMerchantSignature(request, orderReference, orderDate);

        var purchaseRequest = new WayForPayPurchaseRequest
        {
            Amount = request.PaymentRequestDto.Amount,
            Currency = request.PaymentRequestDto.Currency,
            MerchantAccount = _way4PayOptions.Value.MerchantLogin,
            MerchantDomainName = _way4PayOptions.Value.MerchantDomainName,
            OrderDate = orderDate,
            OrderReference = orderReference.ToString(),
            ProductCount = [1],
            ProductName = [PaymentConstants.ProductName],
            ProductPrice = [request.PaymentRequestDto.Amount],
            MerchantSignature = merchantSignature,
            ReturnUrl = request.PaymentRequestDto.ReturnUrl
        };

        if (request.PaymentRequestDto.IsSubscription)
        {
            purchaseRequest.RegularAmount = request.PaymentRequestDto.Amount;
            purchaseRequest.RegularMode = PaymentConstants.ClientSelectedRegularPaymentMode;
            purchaseRequest.RegularOn = true;
        }

        var keyValues = new Dictionary<string, string>
        {
            ["merchantAccount"] = purchaseRequest.MerchantAccount,
            ["merchantDomainName"] = purchaseRequest.MerchantDomainName,
            ["orderReference"] = purchaseRequest.OrderReference,
            ["orderDate"] = purchaseRequest.OrderDate.ToString(),
            ["amount"] = purchaseRequest.Amount.ToString(CultureInfo.InvariantCulture),
            ["currency"] = purchaseRequest.Currency.ToString(),
            ["productName[]"] = purchaseRequest.ProductName[0],
            ["productCount[]"] = purchaseRequest.ProductCount[0].ToString(CultureInfo.InvariantCulture),
            ["productPrice[]"] = purchaseRequest.ProductPrice[0].ToString(CultureInfo.InvariantCulture),
            ["merchantSignature"] = purchaseRequest.MerchantSignature,
        };

        if (purchaseRequest.RegularOn is true)
        {
            keyValues["regularOn"] = "1";
            keyValues["regularAmount"] = (purchaseRequest.RegularAmount ?? purchaseRequest.Amount)
                .ToString(CultureInfo.InvariantCulture);
            keyValues["regularMode"] = purchaseRequest.RegularMode!;
        }

        if (!string.IsNullOrWhiteSpace(purchaseRequest.ReturnUrl))
        {
            keyValues["returnUrl"] = purchaseRequest.ReturnUrl;
        }

        var client = _httpClientFactory.CreateClient("Way4PayClient");
        using var httpRequestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(_way4PayOptions.Value.ApiUrl),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(keyValues)
        };

        try
        {
            using var response = await client.SendAsync(httpRequestMessage, cancellationToken);

            if (response.StatusCode is HttpStatusCode.Found or HttpStatusCode.SeeOther or HttpStatusCode.Moved)
            {
                var paymentUrl = response.Headers.Location?.ToString();
                if (!string.IsNullOrEmpty(paymentUrl))
                {
                    return Result.Ok(new PaymentResponseDto
                    {
                        PaymentUrl = paymentUrl
                    });
                }
            }

            _logger.LogWarning(
                "WayForPay payment request failed with status code {StatusCode}",
                response.StatusCode);
            return Result.Fail(PaymentConstants.UnableToConductDonation);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to communicate with WayForPay while initiating a donation");
            return Result.Fail(PaymentConstants.UnableToConductDonation);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, PaymentConstants.PaymentRequestWasCancelledOrTimedOut);
            return Result.Fail(PaymentConstants.PaymentRequestWasCancelledOrTimedOut);
        }
    }

    [SuppressMessage(
        "SonarLint",
        "S4790",
        Justification = "This is intentional because WayForPay API expects exactly this hashing mechanism")]
    private string GenerateMerchantSignature(PaymentCommand request, Guid orderReference, long orderDate)
    {
        var concatenatedValues = string.Join(
            ';',
            _way4PayOptions.Value.MerchantLogin,
            _way4PayOptions.Value.MerchantDomainName,
            orderReference,
            orderDate,
            request.PaymentRequestDto.Amount.ToString(CultureInfo.InvariantCulture),
            request.PaymentRequestDto.Currency,
            PaymentConstants.ProductName,
            1,
            request.PaymentRequestDto.Amount.ToString(CultureInfo.InvariantCulture));

        var secretKeyBytes = Encoding.UTF8.GetBytes(_way4PayOptions.Value.MerchantSecretKey);
        var signatureBytes = Encoding.UTF8.GetBytes(concatenatedValues);

        using var hmac = new HMACMD5(secretKeyBytes);

        var bytes = hmac.ComputeHash(signatureBytes);
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
