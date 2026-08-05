using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Update;

namespace VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;

public class UpdatePartnersPageBannerLocalizationValidator : AbstractValidator<UpdatePartnersPageBannerLocalizationCommand>
{
    public UpdatePartnersPageBannerLocalizationValidator(BasePartnersPageBannerLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdatePartnersPageBannerLocalizationDto).SetValidator(baseValidator);
    }
}
