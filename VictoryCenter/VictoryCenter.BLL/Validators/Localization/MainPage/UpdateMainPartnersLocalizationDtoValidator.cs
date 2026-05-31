using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class UpdateMainPartnersLocalizationDtoValidator : AbstractValidator<UpdateMainPartnersLocalizationDto>
{
    public UpdateMainPartnersLocalizationDtoValidator(BaseMainPageLocalizationDtoValidator baseValidator)
    {
        Include(baseValidator);
    }
}
