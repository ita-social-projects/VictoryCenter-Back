using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners.Dto;

public class CreatePartnerSectionValidator : BasePartnerSectionValidator<CreatePartnersSectionDto>
{
    public CreatePartnerSectionValidator()
    {
        RuleFor(x => x.Partners)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(CreatePartnersSectionDto.Partners)))
            .Must(partners => partners.Count <= PartnerConstants.PartnersSectionPartnersMaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(CreatePartnersSectionDto.Partners), PartnerConstants.PartnersSectionPartnersMaxCount));

        RuleForEach(x => x.Partners)
            .SetValidator(new CreatePartnerValidator());
    }
}
