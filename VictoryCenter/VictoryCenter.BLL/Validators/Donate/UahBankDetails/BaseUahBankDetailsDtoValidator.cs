using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.UahBankDetails;

public class BaseUahBankDetailsDtoValidator : AbstractValidator<BaseUahBankDetailsDto>
{
    public BaseUahBankDetailsDtoValidator()
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

        RuleFor(dto => dto.UkrainianIban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.UkrainianIban)))
            .MaximumLength(UahBankDetailsConstants.UkrainianIban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.UkrainianIban), UahBankDetailsConstants.UkrainianIban.MaxLength))
            .MinimumLength(UahBankDetailsConstants.UkrainianIban.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.UkrainianIban), UahBankDetailsConstants.UkrainianIban.MinLength))
            .Matches(UahBankDetailsConstants.UahIbanExpression)
            .WithMessage(UahBankDetailsConstants.UkrainianIbanMustStartWithUaFollowedByDigits);

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
