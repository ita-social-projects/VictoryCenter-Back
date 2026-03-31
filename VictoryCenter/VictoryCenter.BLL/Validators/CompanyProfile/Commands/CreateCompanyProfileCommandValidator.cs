using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;
using VictoryCenter.BLL.Validators.CompanyProfile.Dto;

namespace VictoryCenter.BLL.Validators.CompanyProfile.Commands;

public class CreateCompanyProfileCommandValidator : AbstractValidator<CreateCompanyProfileCommand>
{
    public CreateCompanyProfileCommandValidator()
    {
        RuleFor(x => x.CreateCompanyProfileDto)
            .NotNull()
            .SetValidator(new BaseCompanyProfileDtoValidator<
                CreateCompanyProfileContactsDto,
                CreateCompanyProfileRequisiteDto,
                CreateSocialLinkDto>());
    }
}
