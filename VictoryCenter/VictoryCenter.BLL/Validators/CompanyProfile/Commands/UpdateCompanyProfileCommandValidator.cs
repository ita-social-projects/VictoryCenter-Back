using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;
using VictoryCenter.BLL.Validators.CompanyProfile.Dto;

namespace VictoryCenter.BLL.Validators.CompanyProfile.Commands;

public class UpdateCompanyProfileCommandValidator : AbstractValidator<UpdateCompanyProfileCommand>
{
    public UpdateCompanyProfileCommandValidator()
    {
        RuleFor(x => x.UpdateCompanyProfileDto)
            .NotNull()
            .SetValidator(new BaseCompanyProfileDtoValidator<
                UpdateCompanyProfileContactDto,
                UpdateCompanyProfileRequisiteDto,
                UpdateSocialLinkDto>())
            .When(x => x.UpdateCompanyProfileDto is not null);

        When(x => x.UpdateCompanyProfileDto is not null, () =>
        {
            RuleForEach(x => x.UpdateCompanyProfileDto.Contacts.Localization)
                .ChildRules(l => l
                    .RuleFor(x => x.LanguageId)
                    .GreaterThan(0)
                    .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateCompanyProfileContactLocalizationDto.LanguageId))))
                .When(x => x.UpdateCompanyProfileDto.Contacts is not null);

            RuleForEach(x => x.UpdateCompanyProfileDto.Requisites.Localization)
                .ChildRules(l => l
                    .RuleFor(x => x.LanguageId)
                    .GreaterThan(0)
                    .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateCompanyProfileRequisiteLocalizationDto.LanguageId))))
                .When(x => x.UpdateCompanyProfileDto.Requisites is not null);
        });
    }
}
