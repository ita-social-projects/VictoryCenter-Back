using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class UpdateMainPageLocalizationDtoValidator : AbstractValidator<UpdateMainPageLocalizationDto>
{
    public UpdateMainPageLocalizationDtoValidator(
        BaseMainPageLocalizationDtoValidator baseValidator,
        UpdateMainAboutUsLocalizationDtoValidator mainAboutUsValidator,
        UpdateMainPartnersLocalizationDtoValidator mainPartnersValidator,
        UpdateMainDonationsLocalizationDtoValidator mainDonationsValidator)
    {
        Include(baseValidator);

        RuleFor(x => x.MainAboutUs)
            .SetValidator(mainAboutUsValidator!)
            .When(x => x.MainAboutUs != null);

        RuleFor(x => x.MainPartners)
            .SetValidator(mainPartnersValidator!)
            .When(x => x.MainPartners != null);

        RuleFor(x => x.MainDonations)
            .SetValidator(mainDonationsValidator!)
            .When(x => x.MainDonations != null);
    }
}
