using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;

public class BaseForeignBankDetailsDtoValidator : AbstractValidator<BaseForeignBankDetailsDto>
{
    public BaseForeignBankDetailsDtoValidator()
    {
        RuleFor(dto => dto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)))
            .MaximumLength(ForeignBankDetailsConstants.Swift.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Swift),
                    ForeignBankDetailsConstants.Swift.MaxLength))
            .MinimumLength(ForeignBankDetailsConstants.Swift.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Swift),
                    ForeignBankDetailsConstants.Swift.MinLength))
            .Matches(ForeignBankDetailsConstants.SwiftExpression)
            .WithMessage(ForeignBankDetailsConstants.SwiftMustContainOnlyLettersAndDigits);

        RuleFor(dto => dto.UkrainianIban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.UkrainianIban)))
            .MaximumLength(ForeignBankDetailsConstants.UkrainianIban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.UkrainianIban),
                    ForeignBankDetailsConstants.UkrainianIban.MaxLength))
            .MinimumLength(ForeignBankDetailsConstants.UkrainianIban.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.UkrainianIban),
                    ForeignBankDetailsConstants.UkrainianIban.MinLength))
            .Matches(ForeignBankDetailsConstants.UahIbanExpression)
            .WithMessage(ForeignBankDetailsConstants.UkrainianIbanMustStartWithUaFollowedByDigits);

        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Name)))
            .MaximumLength(ForeignBankDetailsConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Name),
                    ForeignBankDetailsConstants.NameMaxLength));

        RuleFor(dto => dto.Receiver)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Receiver)))
            .MaximumLength(ForeignBankDetailsConstants.ReceiverMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Receiver),
                    ForeignBankDetailsConstants.ReceiverMaxLength));

        RuleFor(dto => dto.Address)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Address)))
            .MaximumLength(ForeignBankDetailsConstants.AddressMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Address),
                    ForeignBankDetailsConstants.AddressMaxLength));
    }
}
