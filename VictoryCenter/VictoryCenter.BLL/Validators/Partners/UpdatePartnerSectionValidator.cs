using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class UpdatePartnerSectionValidator : BasePartnerSectionValidator<UpdatePartnersSectionDto>
{
    public UpdatePartnerSectionValidator()
    {
        RuleFor(x => x.Partners)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(UpdatePartnersSectionDto.Partners)))
            .Must(partners => partners.Count <= PartnerConstants.PartnersMaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(CreatePartnersSectionDto.Partners), PartnerConstants.PartnersMaxCount));

        RuleForEach(x => x.Partners)
            .SetValidator(new UpdatePartnerValidator());
    }
}
