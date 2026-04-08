using FluentValidation;
using VictoryCenter.BLL.Constants;
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

        RuleFor(x => x.ImpactStatistics)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateMainPageDto.ImpactStatistics)))
            .Must(s => s.Count <= MainPageConstants.ImpactStatistic.MaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(CreateMainPageDto.ImpactStatistics), MainPageConstants.ImpactStatistic.MaxCount));

        RuleForEach(x => x.ImpactStatistics)
            .SetValidator(new CreateImpactStatisticDtoValidator());
    }
}
