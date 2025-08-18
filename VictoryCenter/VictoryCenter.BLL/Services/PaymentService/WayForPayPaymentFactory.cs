using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Public.Payment.Common;
using VictoryCenter.BLL.Commands.Public.Payment.WayForPay;
using VictoryCenter.BLL.DTOs.Public.Payment;
using VictoryCenter.BLL.DTOs.Public.Payment.Common;
using VictoryCenter.BLL.Interfaces.PaymentService;
using VictoryCenter.BLL.Options.Payment;

namespace VictoryCenter.BLL.Services.PaymentService;

public class WayForPayPaymentFactory : IPaymentFactory
{
    private readonly IOptions<WayForPayOptions> _way4PayOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WayForPayPaymentCommandHandler> _logger;

    public WayForPayPaymentFactory(IOptions<WayForPayOptions> way4PayOptions, IHttpClientFactory httpClientFactory, ILogger<WayForPayPaymentCommandHandler> logger)
    {
        _way4PayOptions = way4PayOptions;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public PaymentSystem PaymentSystem => PaymentSystem.WayForPay;

    public IPaymentCommandHandler<PaymentCommand, Result<PaymentResponseDto>> GetRequestHandler()
    {
        return new WayForPayPaymentCommandHandler(_way4PayOptions, _httpClientFactory, _logger);
    }
}
