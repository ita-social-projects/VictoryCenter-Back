using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;

public class CreatePartnersPageBannerLocalizationValidator : AbstractValidator<CreatePartnersPageBannerLocalizationCommand>
{
    public CreatePartnersPageBannerLocalizationValidator(BasePartnersPageBannerLocalizationValidator baseValidator)
    {
        RuleFor(x => x.CreatePartnersPageBannerLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreatePartnersPageBannerLocalizationDto>());

        RuleFor(x => x.CreatePartnersPageBannerLocalizationDto).SetValidator(baseValidator);
    }
}
