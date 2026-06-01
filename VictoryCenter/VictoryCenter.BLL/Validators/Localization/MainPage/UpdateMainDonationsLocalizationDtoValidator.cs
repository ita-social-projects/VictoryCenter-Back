using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class UpdateMainDonationsLocalizationDtoValidator : AbstractValidator<UpdateMainDonationsLocalizationDto>
{
    public UpdateMainDonationsLocalizationDtoValidator(BaseMainPageLocalizationDtoValidator baseValidator)
    {
        Include(baseValidator);
    }
}
