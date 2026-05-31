using VictoryCenter.BLL.DTOs.Admin.MainPages;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class CreateMainPageDtoValidator : BaseMainPageDtoValidator<CreateMainPageDto>
{
    public CreateMainPageDtoValidator()
    {
        When(x => x.MainAboutUs is not null, () =>
        {
            RuleFor(x => x.MainAboutUs!)
                .SetValidator(new CreateMainAboutUsDtoValidator());
        });

        When(x => x.MainPartners is not null, () =>
        {
            RuleFor(x => x.MainPartners!)
                .SetValidator(new CreateMainPartnersDtoValidator());
        });

        When(x => x.ImpactStatistics is not null, () =>
        {
            RuleFor(x => x.ImpactStatistics!)
                .SetValidator(new CreateImpactStatisticDtoValidator());
        });
    }
}
