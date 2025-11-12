using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;

public class ForeignBankDetailsDtoValidator<T> : AbstractValidator<T>
    where T : IForeignBankDetails
{
    public ForeignBankDetailsDtoValidator()
    {
        RuleFor(dto => dto.Swift)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)))
            .MaximumLength(ForeignBankDetailsConstants.Swift.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsConstants.Swift),
                    ForeignBankDetailsConstants.Swift.MaxLength))
            .MinimumLength(ForeignBankDetailsConstants.Swift.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(ForeignBankDetailsConstants.Swift),
                    ForeignBankDetailsConstants.Swift.MinLength));

        RuleFor(dto => dto.Iban)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Iban)))
            .MaximumLength(ForeignBankDetailsConstants.Iban.MaxLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsConstants.Iban),
                    ForeignBankDetailsConstants.Iban.MaxLength))
            .MinimumLength(ForeignBankDetailsConstants.Iban.MinLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(ForeignBankDetailsConstants.Iban),
                    ForeignBankDetailsConstants.Iban.MinLength))
            .Matches(ForeignBankDetailsConstants.UahIbanExpression)
            .WithMessage(ForeignBankDetailsConstants.IbanMustStartWithUaFollowedByDigits);

        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Name)));

        RuleFor(dto => dto.Receiver)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Receiver)));

        RuleFor(dto => dto.Address)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Address)));
    }
}
