using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class UpdatePartnerSectionValidator : BasePartnerSectionValidator<UpdatePartnersSectionDto>
{
    public UpdatePartnerSectionValidator()
    {
        RuleFor(x => x.PartnersToUpdate)
            .NotNull()
            .Must(partners => partners.Count <= PartnerConstants.PartnersMaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(CreatePartnersSectionDto.Partners), PartnerConstants.PartnersMaxCount));

        RuleForEach(x => x.PartnersToUpdate)
            .SetValidator(new UpdatePartnerValidator());

        RuleFor(x => x.PartnerIdsToDelete)
            .NotNull();
    }
}
