using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class BaseMainPageLocalizationDtoValidator : AbstractValidator<BaseMainPageLocalizationDto>
{
    public BaseMainPageLocalizationDtoValidator()
    {
        this.AddTitleAndDescriptionRules(MainPageConstants.Localization.ValidationTitleBlockDescriptionRules.MaxLen);
    }
}
