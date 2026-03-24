using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;

namespace VictoryCenter.BLL.Validators.CompanyProfile.Dto;

public class BaseCompanyProfileContactDtoValidator : AbstractValidator<BaseCompanyProfileContactsDto>
{
    public BaseCompanyProfileContactDtoValidator()
    {
        RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileContactsDto.Phone)))
            .MaximumLength(CompanyProfileConstants.Phone.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseCompanyProfileContactsDto.Phone), CompanyProfileConstants.Phone.MaxLength));

        RuleFor(x => x.Address)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileContactsDto.Address)))
            .MaximumLength(CompanyProfileConstants.Address.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseCompanyProfileContactsDto.Address), CompanyProfileConstants.Address.MaxLength));

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileContactsDto.Email)))
            .MaximumLength(CompanyProfileConstants.Email.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseCompanyProfileContactsDto.Email), CompanyProfileConstants.Email.MaxLength))
            .EmailAddress()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(BaseCompanyProfileContactsDto.Email)));

        RuleFor(x => x.CorrespondenceEmail)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(BaseCompanyProfileContactsDto.CorrespondenceEmail)))
            .MaximumLength(CompanyProfileConstants.CorrespondenceEmail.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseCompanyProfileContactsDto.CorrespondenceEmail), CompanyProfileConstants.CorrespondenceEmail.MaxLength))
            .EmailAddress()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(BaseCompanyProfileContactsDto.CorrespondenceEmail)));

        RuleFor(x => x.Motto)
            .MaximumLength(CompanyProfileConstants.Motto.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseCompanyProfileContactsDto.Motto), CompanyProfileConstants.Motto.MaxLength))
            .When(x => x.Motto is not null);
    }
}
