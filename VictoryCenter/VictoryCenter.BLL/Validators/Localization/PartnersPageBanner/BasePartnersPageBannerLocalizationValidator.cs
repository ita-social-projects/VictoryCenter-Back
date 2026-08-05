using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;

namespace VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;

public class BasePartnersPageBannerLocalizationValidator : AbstractValidator<UpdatePartnersPageBannerLocalizationDto>
{
    public BasePartnersPageBannerLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerLocalizationDto.Title)))
            .MinimumLength(PartnerConstants.PartnersPageBannerTitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdatePartnersPageBannerLocalizationDto.Title), PartnerConstants.PartnersPageBannerTitleMinLength))
            .MaximumLength(PartnerConstants.PartnersPageBannerTitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdatePartnersPageBannerLocalizationDto.Title), PartnerConstants.PartnersPageBannerTitleMaxLength));

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdatePartnersPageBannerLocalizationDto.Description)))
            .MinimumLength(PartnerConstants.PartnersPageBannerDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdatePartnersPageBannerLocalizationDto.Description), PartnerConstants.PartnersPageBannerDescriptionMinLength))
            .MaximumLength(PartnerConstants.PartnersPageBannerDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdatePartnersPageBannerLocalizationDto.Description), PartnerConstants.PartnersPageBannerDescriptionMaxLength));
    }
}
