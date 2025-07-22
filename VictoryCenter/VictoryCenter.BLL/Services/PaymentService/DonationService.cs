using FluentResults;
using FluentValidation;
using VictoryCenter.BLL.Commands.Donation.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment.Donation;
using VictoryCenter.BLL.Factories.Donation.Interfaces;
using VictoryCenter.BLL.Interfaces.PaymentService;

namespace VictoryCenter.BLL.Services.PaymentService;

public class DonationService : IDonationService
{
    private readonly IEnumerable<IDonationFactory> _donationFactories;
    private readonly IValidator<DonationRequestDto> _validator;

    public DonationService(IEnumerable<IDonationFactory> donationFactories, IValidator<DonationRequestDto> validator)
    {
        _donationFactories = donationFactories;
        _validator = validator;
    }

    public async Task<Result<DonationResponseDto>> CreateDonation(DonationRequestDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));
        }

        var donationFactory = _donationFactories.SingleOrDefault(df => df.PaymentSystem == request.PaymentSystem);
        if (donationFactory is null)
        {
            return Result.Fail(PaymentConstants.ChosenPaymentSystemIsNotSupported);
        }

        var commandHandler = donationFactory.GetRequestHandler();

        return await commandHandler.Handle(new DonationCommand(request), cancellationToken);
    }
}
