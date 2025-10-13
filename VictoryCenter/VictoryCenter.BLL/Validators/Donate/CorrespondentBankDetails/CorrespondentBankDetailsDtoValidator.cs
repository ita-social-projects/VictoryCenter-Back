using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;
public class CorrespondentBankDetailsDtoValidator<T> : AbstractValidator<T>
    where T : CreateCorrespondentBankDetailsDto
{
    public CorrespondentBankDetailsDtoValidator()
    {
        RuleFor(dto => dto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)))
            .MaximumLength(CorrespondentBankDetailsConstants.Swift.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MaxLength))
            .MinimumLength(CorrespondentBankDetailsConstants.Swift.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MinLength));

        RuleFor(dto => dto.Iban)
            .MaximumLength(CorrespondentBankDetailsConstants.Iban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MaxLength))
            .MinimumLength(CorrespondentBankDetailsConstants.Iban.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MinLength))
            .Matches(CorrespondentBankDetailsConstants.OnlyDigits)
            .WithMessage(CorrespondentBankDetailsConstants.OnlyDigitsMessage)
            .When(dto => !string.IsNullOrEmpty(dto.Iban));
    }
}
