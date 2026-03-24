using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;
using VictoryCenter.BLL.Validators.CompanyProfile.Dto;

namespace VictoryCenter.BLL.Validators.CompanyProfile.Commands;

public class CreateCompanyProfileCommandValidator : AbstractValidator<CreateCompanyProfileCommand>
{
    public CreateCompanyProfileCommandValidator()
    {
        RuleFor(x => x.CreateCompanyProfileDto)
            .NotNull();

        RuleFor(x => x.CreateCompanyProfileDto.Contacts)
            .NotNull()
            .SetValidator(new BaseCompanyProfileContactDtoValidator())
            .When(x => x.CreateCompanyProfileDto is not null);

        RuleFor(x => x.CreateCompanyProfileDto.Requisites)
            .NotNull()
            .SetValidator(new BaseCompanyProfileRequisiteDtoValidator())
            .When(x => x.CreateCompanyProfileDto is not null);

        RuleForEach(x => x.CreateCompanyProfileDto.SocialLinks)
            .SetValidator(new BaseSocialLinkDtoValidator())
            .When(x => x.CreateCompanyProfileDto is not null);
    }
}
