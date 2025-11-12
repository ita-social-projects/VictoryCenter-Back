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
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsConstants.Swift),
                    CorrespondentBankDetailsConstants.Swift.MaxLength))
            .MinimumLength(CorrespondentBankDetailsConstants.Swift.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsConstants.Swift),
                    CorrespondentBankDetailsConstants.Swift.MinLength));

        RuleFor(dto => dto.Iban)
            .MaximumLength(CorrespondentBankDetailsConstants.Iban.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CorrespondentBankDetailsConstants.Iban),
                CorrespondentBankDetailsConstants.Iban.MaxLength));

        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Name)));

        RuleFor(dto => dto.Account)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Account)));
    }
}
