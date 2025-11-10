using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.UahBankDetails;

public class UahBankDetailsDtoValidator<T> : AbstractValidator<T>
    where T : CreateUahBankDetailsDto
{
    public UahBankDetailsDtoValidator()
    {
        RuleFor(dto => dto.Edrpou)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Edrpou)))
            .Matches(UahBankDetailsConstants.OnlyDigits)
            .WithMessage(UahBankDetailsConstants.OnlyDigitsMessage);

        RuleFor(dto => dto.Iban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Iban)));

        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Name)));

        RuleFor(dto => dto.Receiver)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Receiver)));

        RuleFor(dto => dto.PaymentPurpose)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.PaymentPurpose)));
    }
}
