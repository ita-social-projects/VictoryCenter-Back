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
            .MaximumLength(UahBankDetailsConstants.Edrpou.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MaxLength))
            .MinimumLength(UahBankDetailsConstants.Edrpou.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MinLength))
            .Matches(ErrorMessagesConstants.OnlyDigitsExpression)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustContainOnlyDigits(nameof(UahBankDetailsDto.Edrpou)));

        RuleFor(dto => dto.Iban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Iban)))
            .MaximumLength(UahBankDetailsConstants.Iban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MaxLength))
            .MinimumLength(UahBankDetailsConstants.Iban.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MinLength))
            .Matches(UahBankDetailsConstants.UahIbanExpression)
            .WithMessage(UahBankDetailsConstants.IbanMustStartWithUaFollowedByDigits);

        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Name)))
            .MaximumLength(UahBankDetailsConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UahBankDetailsDto.Name),
                    UahBankDetailsConstants.NameMaxLength));

        RuleFor(dto => dto.Receiver)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Receiver)))
            .MaximumLength(UahBankDetailsConstants.ReceiverMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UahBankDetailsDto.Receiver),
                    UahBankDetailsConstants.ReceiverMaxLength));

        RuleFor(dto => dto.PaymentPurpose)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.PaymentPurpose)))
            .MaximumLength(UahBankDetailsConstants.PaymentPurposeMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UahBankDetailsDto.PaymentPurpose),
                    UahBankDetailsConstants.PaymentPurposeMaxLength));
    }
}
