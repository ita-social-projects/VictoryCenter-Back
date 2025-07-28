using FluentResults;
using FluentValidation;
using VictoryCenter.BLL.Commands.Payment.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Payment.Common;
using VictoryCenter.BLL.Factories.Payment.Interfaces;
using VictoryCenter.BLL.Interfaces.PaymentService;

namespace VictoryCenter.BLL.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly IEnumerable<IPaymentFactory> _donationFactories;
    private readonly IValidator<PaymentRequestDto> _validator;

    public PaymentService(IEnumerable<IPaymentFactory> donationFactories, IValidator<PaymentRequestDto> validator)
    {
        _donationFactories = donationFactories;
        _validator = validator;
    }

    public async Task<Result<PaymentResponseDto>> CreatePayment(PaymentRequestDto request, CancellationToken cancellationToken)
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

        return await commandHandler.Handle(new PaymentCommand(request), cancellationToken);
    }
}
