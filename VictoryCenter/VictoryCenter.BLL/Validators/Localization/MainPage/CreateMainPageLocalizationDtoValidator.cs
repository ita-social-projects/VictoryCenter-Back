using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class CreateMainPageLocalizationDtoValidator : AbstractValidator<CreateMainPageLocalizationDto>
{
    public CreateMainPageLocalizationDtoValidator(
        BaseMainPageLocalizationDtoValidator baseValidator,
        CreateMainAboutUsLocalizationDtoValidator mainAboutUsValidator,
        CreateMainPartnersLocalizationDtoValidator mainPartnersValidator,
        CreateMainDonationsLocalizationDtoValidator mainDonationsValidator)
    {
        RuleFor(x => x)
            .SetValidator(new LocalizationIdentityValidator<CreateMainPageLocalizationDto>());

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
