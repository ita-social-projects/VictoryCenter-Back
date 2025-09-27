using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Validators.Donate;
public class UpdateUahBankDetailsCommandValidator : AbstractValidator<UpdateUahBankDetailsCommand>
{
    public UpdateUahBankDetailsCommandValidator()
    {
        RuleFor(command => command.UpdateUahBankDetailsDto.Edrpou)
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

        RuleFor(command => command.UpdateUahBankDetailsDto.Iban)
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
