using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Validators.Donate;
public class CreateUahBankDetailsCommandValidator : AbstractValidator<CreateUahBankDetailsCommand>
{
    public CreateUahBankDetailsCommandValidator()
    {
        RuleFor(command => command.CreateUahBankDetailsDto.Edrpou)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Edrpou)))
            .MaximumLength(UahBankDetailsConstants.Edrpou.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MaxLength))
            .MinimumLength(UahBankDetailsConstants.Edrpou.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MinLength))
            .Matches(UahBankDetailsConstants.OnlyDigits)
            .WithMessage(UahBankDetailsConstants.OnlyDigitsMessage);

        RuleFor(command => command.CreateUahBankDetailsDto.Iban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Iban)))
            .MaximumLength(UahBankDetailsConstants.Iban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MaxLength))
            .MinimumLength(UahBankDetailsConstants.Iban.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MinLength))
            .Matches(UahBankDetailsConstants.OnlyDigits)
            .WithMessage(UahBankDetailsConstants.OnlyDigitsMessage);
    }
}
