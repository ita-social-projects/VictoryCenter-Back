using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Payment.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment.Common;
using VictoryCenter.BLL.DTOs.Payment.WayForPay;
using VictoryCenter.BLL.Options.Payment;

namespace VictoryCenter.BLL.Commands.Payment.WayForPay;

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

        var purchaseRequest = new WayForPayPurchaseRequest()
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

        // regular payment is going to be supported in the future
        // if (request.Request.IsSubscription)
        // {
        //     purchaseRequest.RegularBehavior = PaymentConstants.RegularPaymentBehaviour;
        //     purchaseRequest.RegularAmount = request.Request.Amount;
        //     purchaseRequest.RegularMode = PaymentConstants.RegularPaymentMode;
        //     purchaseRequest.RegularOn = true;
        // }

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

        // regular payment is going to be supported in the future
        // if (purchaseRequest.RegularOn.HasValue && purchaseRequest.RegularOn.Value)
        // {
        //     keyValues["regularOn"] = "1";
        //     keyValues["regularAmount"] = purchaseRequest.RegularAmount?.ToString(CultureInfo.InvariantCulture) ?? purchaseRequest.Amount.ToString(CultureInfo.InvariantCulture);
        //     keyValues["regularMode"] = purchaseRequest.RegularMode!;
        //     keyValues["regularBehavior"] = purchaseRequest.RegularBehavior!;
        //     keyValues["regularCount"] = PaymentConstants.RegularPaymentCount;
        // }

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

        try
        {
            var response = await client.SendAsync(httpRequestMessage, cancellationToken);

            if (response.StatusCode is HttpStatusCode.Found or HttpStatusCode.SeeOther or HttpStatusCode.Moved)
            {
                var paymentUrl = response.Headers.Location?.ToString();
                if (!string.IsNullOrEmpty(paymentUrl))
                {
                    return Result.Ok(new PaymentResponseDto()
                    {
                        PaymentUrl = paymentUrl
                    });
                }
            }

            return Result.Fail(response.ReasonPhrase ?? PaymentConstants.PaymentRequestFailedWithStatus(response.StatusCode));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error occured when processing payment request: {ErrorMessage}", ex.Message);
            return Result.Fail(PaymentConstants.FailedToCommunicateWithPaymentGateway(ex.Message));
        }
        catch (TaskCanceledException ex)
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
            request.Request.Amount.ToString(CultureInfo.InvariantCulture),
            request.Request.Currency,
            PaymentConstants.ProductName,
            1,
            request.Request.Amount.ToString(CultureInfo.InvariantCulture));

        var secretKeyBytes = Encoding.UTF8.GetBytes(_way4PayOptions.Value.MerchantSecretKey);
        var signatureBytes = Encoding.UTF8.GetBytes(concatenatedValues);

        // Using HMACMD5 is required by WayForPay API specification. Suppressing SonarQube rule S5547.
        // NOSONAR
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
