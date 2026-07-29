using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

namespace VictoryCenter.BLL.Validators.Localization.PartnerSections;

public class PartnerLocalizationItemValidator : AbstractValidator<UpdatePartnerLocalizationItemDto>
{
    public PartnerLocalizationItemValidator()
    {
        RuleFor(x => x.PartnerId)
            .GreaterThan(0).WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdatePartnerLocalizationItemDto.PartnerId)));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnerLocalizationItemDto.Description)))
            .MinimumLength(PartnerConstants.PartnerDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdatePartnerLocalizationItemDto.Description), PartnerConstants.PartnerDescriptionMinLength))
            .MaximumLength(PartnerConstants.PartnerDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdatePartnerLocalizationItemDto.Description), PartnerConstants.PartnerDescriptionMaxLength));
    }
}
