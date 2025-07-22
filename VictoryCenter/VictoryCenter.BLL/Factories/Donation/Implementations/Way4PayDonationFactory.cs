using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Donation.Common;
using VictoryCenter.BLL.Commands.Donation.Way4Pay;
using VictoryCenter.BLL.DTOs.Payment;
using VictoryCenter.BLL.DTOs.Payment.Donation;
using VictoryCenter.BLL.Factories.Donation.Interfaces;
using VictoryCenter.BLL.Options.Donation;

namespace VictoryCenter.BLL.Factories.Donation.Implementations;

public class Way4PayDonationFactory : IDonationFactory
{
    private readonly IOptions<Way4PayOptions> _way4PayOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<Way4PayDonationCommandHandler> _logger;

    public Way4PayDonationFactory(IOptions<Way4PayOptions> way4PayOptions, IHttpClientFactory httpClientFactory, ILogger<Way4PayDonationCommandHandler> logger)
    {
        _way4PayOptions = way4PayOptions;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public PaymentSystem PaymentSystem => PaymentSystem.Way4Pay;

    public IDonationCommandHandler<DonationCommand, Result<DonationResponseDto>> GetRequestHandler()
    {
        return new Way4PayDonationCommandHandler(_way4PayOptions, _httpClientFactory, _logger);
    }
}
