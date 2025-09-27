using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Validators.Donate;
public class UpdateForeignBankDetailsCommandValidator : AbstractValidator<UpdateForeignBankDetailsCommand>
{
    public UpdateForeignBankDetailsCommandValidator()
    {
        RuleFor(command => command.UpdateForeignBankDetailsDto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)))
            .MaximumLength(ForeignBankDetailsConstants.Swift.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Swift), ForeignBankDetailsConstants.Swift.MaxLength))
            .MinimumLength(ForeignBankDetailsConstants.Swift.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Swift), ForeignBankDetailsConstants.Swift.MinLength));

        RuleFor(command => command.UpdateForeignBankDetailsDto.Iban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Iban)))
            .MaximumLength(ForeignBankDetailsConstants.Iban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Iban), ForeignBankDetailsConstants.Iban.MaxLength))
            .MinimumLength(ForeignBankDetailsConstants.Iban.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Iban), ForeignBankDetailsConstants.Iban.MinLength))
            .Matches(ForeignBankDetailsConstants.OnlyDigits)
            .WithMessage(ForeignBankDetailsConstants.OnlyDigitsMessage);

        RuleForEach(command => command.UpdateForeignBankDetailsDto.CorrespondentBanks)
            .SetValidator(new UpdateCorrespondentBankDetailsDtoValidator());
    }
}
