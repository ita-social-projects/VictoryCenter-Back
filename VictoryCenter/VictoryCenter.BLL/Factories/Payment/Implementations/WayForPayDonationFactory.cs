using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Payment.Common;
using VictoryCenter.BLL.Commands.Payment.WayForPay;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Common;
using VictoryCenter.BLL.Factories.Payment.Interfaces;
using VictoryCenter.BLL.Options.Payment;

namespace VictoryCenter.BLL.Factories.Payment.Implementations;

public class WayForPayDonationFactory : IDonationFactory
{
    private readonly IOptions<WayForPayOptions> _way4PayOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WayForPayPaymentCommandHandler> _logger;

    public WayForPayDonationFactory(IOptions<WayForPayOptions> way4PayOptions, IHttpClientFactory httpClientFactory, ILogger<WayForPayPaymentCommandHandler> logger)
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
