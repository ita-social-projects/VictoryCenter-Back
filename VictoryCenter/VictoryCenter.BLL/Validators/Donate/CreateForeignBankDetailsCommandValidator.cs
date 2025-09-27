using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Validators.Donate;
public class CreateForeignBankDetailsCommandValidator : AbstractValidator<CreateForeignBankDetailsCommand>
{
    public CreateForeignBankDetailsCommandValidator()
    {
        RuleFor(command => command.CreateForeignBankDetailsDto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)))
            .MaximumLength(ForeignBankDetailsConstants.Swift.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Swift), ForeignBankDetailsConstants.Swift.MaxLength))
            .MinimumLength(ForeignBankDetailsConstants.Swift.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Swift), ForeignBankDetailsConstants.Swift.MinLength));

        RuleFor(command => command.CreateForeignBankDetailsDto.Iban)
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

        RuleForEach(command => command.CreateForeignBankDetailsDto.CorrespondentBanks)
            .SetValidator(new CreateCorrespondentBankDetailsDtoValidator());
    }
}
