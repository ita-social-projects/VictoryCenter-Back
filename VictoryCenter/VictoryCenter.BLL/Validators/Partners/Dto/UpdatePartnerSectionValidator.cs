using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners.Dto;

public class UpdatePartnerSectionValidator : BasePartnerSectionValidator<UpdatePartnersSectionDto>
{
    public UpdatePartnerSectionValidator()
    {
        RuleFor(x => x.PartnersToCreate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(partners => partners.Count <= PartnerConstants.PartnersSectionPartnersMaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdatePartnersSectionDto.PartnersToCreate), PartnerConstants.PartnersSectionPartnersMaxCount));
        RuleForEach(x => x.PartnersToCreate)
            .SetValidator(new CreatePartnerValidator());

        RuleFor(x => x.PartnersToUpdate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(partners => partners.Count <= PartnerConstants.PartnersSectionPartnersMaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdatePartnersSectionDto.PartnersToUpdate), PartnerConstants.PartnersSectionPartnersMaxCount));
        RuleForEach(x => x.PartnersToUpdate)
            .SetValidator(new UpdatePartnerValidator());

        RuleFor(x => x.PartnerIdsToDelete)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(partners => partners.Count <= PartnerConstants.PartnersSectionPartnersMaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdatePartnersSectionDto.PartnersToUpdate), PartnerConstants.PartnersSectionPartnersMaxCount));
    }
}
