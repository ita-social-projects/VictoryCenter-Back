using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;

namespace VictoryCenter.BLL.Validators.CompanyProfile.Dto;

public class BaseCompanyProfileRequisiteDtoValidator : AbstractValidator<BaseCompanyProfileRequisiteDto>
{
    public BaseCompanyProfileRequisiteDtoValidator()
    {
        RuleFor(x => x.Recipient)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileRequisiteDto.Recipient)))
            .MaximumLength(CompanyProfileConstants.Recipient.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseCompanyProfileRequisiteDto.Recipient), CompanyProfileConstants.Recipient.MaxLength));

        RuleFor(x => x.Edrpou)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileRequisiteDto.Edrpou)))
            .Length(CompanyProfileConstants.Edrpou.MinLength, CompanyProfileConstants.Edrpou.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveALengthOfNCharacters(
                nameof(BaseCompanyProfileRequisiteDto.Edrpou), CompanyProfileConstants.Edrpou.MaxLength))
            .Matches(ErrorMessagesConstants.OnlyDigitsExpression)
            .WithMessage(ErrorMessagesConstants.PropertyMustContainOnlyDigits(nameof(BaseCompanyProfileRequisiteDto.Edrpou)));

        RuleFor(x => x.Address)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileRequisiteDto.Address)))
            .MaximumLength(CompanyProfileConstants.Address.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseCompanyProfileRequisiteDto.Address), CompanyProfileConstants.Address.MaxLength));
    }
}
