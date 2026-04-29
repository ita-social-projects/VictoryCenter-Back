using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class UpdateMainPageDtoValidator : BaseMainPageDtoValidator<UpdateMainPageDto>
{
    public UpdateMainPageDtoValidator()
    {
        When(x => x.MainAboutUs is not null, () =>
        {
            RuleFor(x => x.MainAboutUs!)
                .SetValidator((IValidator<UpdateMainAboutUsDto?>)new UpdateMainAboutUsDtoValidator());
        });

        When(x => x.MainPartners is not null, () =>
        {
            RuleFor(x => x.MainPartners!)
                .SetValidator((IValidator<UpdateMainPartnersDto?>)new UpdateMainPartnersDtoValidator());
        });

        When(x => x.ImpactStatistics is not null, () =>
        {
            RuleFor(x => x.ImpactStatistics!)
                .SetValidator(new UpdateImpactStatisticDtoValidator());
        });
    }
}
