using FluentValidation;
using VictoryCenter.BLL.Constants;
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
            .SetValidator(new BaseCompanyProfileContactDtoValidator());

        RuleFor(x => x.Requisites)
            .NotNull()
            .SetValidator(new BaseCompanyProfileRequisiteDtoValidator());

        RuleFor(x => x.SocialLinks)
            .Must(list => list.Count <= CompanyProfileConstants.SocialLinks.MaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BaseCompanyProfileDto<TContacts, TRequisites, TSocialLink>.SocialLinks),
                CompanyProfileConstants.SocialLinks.MaxCount))
            .Must(list => list.Select(sl => sl.SocialPlatform).Distinct().Count() == list.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(BaseCompanyProfileDto<TContacts, TRequisites, TSocialLink>.SocialLinks)));

        RuleForEach(x => x.SocialLinks)
            .SetValidator(new BaseSocialLinkDtoValidator());
    }
}
