using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

namespace VictoryCenter.BLL.Validators.Localization.PartnerSections;

public class BasePartnerSectionLocalizationValidator : AbstractValidator<UpdatePartnerSectionLocalizationDto>
{
    public BasePartnerSectionLocalizationValidator(PartnerLocalizationItemValidator partnerLocalizationItemValidator)
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnerSectionLocalizationDto.Title)))
            .MinimumLength(PartnerConstants.PartnersSectionTitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdatePartnerSectionLocalizationDto.Title), PartnerConstants.PartnersSectionTitleMinLength))
            .MaximumLength(PartnerConstants.PartnersSectionTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdatePartnerSectionLocalizationDto.Title), PartnerConstants.PartnersSectionTitleMaxLength));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnerSectionLocalizationDto.Description)))
            .MinimumLength(PartnerConstants.PartnersSectionDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdatePartnerSectionLocalizationDto.Description), PartnerConstants.PartnersSectionDescriptionMinLength))
            .MaximumLength(PartnerConstants.PartnersSectionDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdatePartnerSectionLocalizationDto.Description), PartnerConstants.PartnersSectionDescriptionMaxLength));

        RuleFor(x => x.Partners)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnerSectionLocalizationDto.Partners)))
            .Must(partners => partners.All(p => p is not null)
                && partners.Select(p => p!.PartnerId).Distinct().Count() == partners.Count)
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdatePartnerSectionLocalizationDto.Partners)));

        RuleForEach(x => x.Partners).SetValidator(partnerLocalizationItemValidator);
    }
}
