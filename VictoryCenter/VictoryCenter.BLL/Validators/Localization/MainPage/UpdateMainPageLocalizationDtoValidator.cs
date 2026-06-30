using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class UpdateMainPageLocalizationDtoValidator : AbstractValidator<UpdateMainPageLocalizationDto>
{
    public UpdateMainPageLocalizationDtoValidator(
        UpdateMainAboutUsLocalizationDtoValidator mainAboutUsValidator,
        UpdateMainPartnersLocalizationDtoValidator mainPartnersValidator,
        UpdateMainDonationsLocalizationDtoValidator mainDonationsValidator)
    {
        this.AddOptionalTitleAndDescriptionRules(MainPageConstants.Localization.TitleBlockDescription.MaxLength);

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
