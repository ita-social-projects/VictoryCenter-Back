using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;

namespace VictoryCenter.BLL.Validators.CompanyProfile.Dto;

public class BaseCompanyProfileDtoValidator<TContacts, TRequisites, TSocialLink>
    : AbstractValidator<BaseCompanyProfileDto<TContacts, TRequisites, TSocialLink>>
    where TContacts : BaseCompanyProfileContactsDto
    where TRequisites : BaseCompanyProfileRequisiteDto
    where TSocialLink : BaseSocialLinkDto
{
    public BaseCompanyProfileDtoValidator()
    {
        RuleFor(x => x.Contacts)
            .NotNull()
            .SetValidator(new BaseCompanyProfileContactDtoValidator())
            .When(x => x.Contacts is not null);

        RuleFor(x => x.Requisites)
            .NotNull()
            .SetValidator(new BaseCompanyProfileRequisiteDtoValidator())
            .When(x => x.Requisites is not null);

        RuleForEach(x => x.SocialLinks)
            .SetValidator(new BaseSocialLinkDtoValidator());
    }
}
