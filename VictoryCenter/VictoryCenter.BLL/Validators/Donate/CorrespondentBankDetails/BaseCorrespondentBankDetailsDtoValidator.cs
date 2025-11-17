using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;

public class BaseCorrespondentBankDetailsDtoValidator : AbstractValidator<BaseCorrespondentBankDetailsDto>
{
    public BaseCorrespondentBankDetailsDtoValidator()
    {
        RuleFor(dto => dto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants
                .PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)))
            .MaximumLength(CorrespondentBankDetailsConstants.Swift.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsDto.Swift),
                    CorrespondentBankDetailsConstants.Swift.MaxLength))
            .MinimumLength(CorrespondentBankDetailsConstants.Swift.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsDto.Swift),
                    CorrespondentBankDetailsConstants.Swift.MinLength));

        RuleFor(dto => dto.Iban)
            .MaximumLength(CorrespondentBankDetailsConstants.Iban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsDto.Iban),
                    CorrespondentBankDetailsConstants.Iban.MaxLength));

        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants
                .PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Name)))
            .MaximumLength(CorrespondentBankDetailsConstants.NameMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsDto.Name),
                    CorrespondentBankDetailsConstants.NameMaxLength));

        RuleFor(dto => dto.Account)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants
                .PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Account)))
            .MaximumLength(CorrespondentBankDetailsConstants.AccountMaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsDto.Account),
                    CorrespondentBankDetailsConstants.AccountMaxLength));
    }
}
