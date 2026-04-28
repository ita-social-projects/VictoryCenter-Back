using FluentValidation;
using VictoryCenter.BLL.Constants;
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

        RuleFor(x => x.ImpactStatistics)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateMainPageDto.ImpactStatistics)))
            .Must(s => s.Count <= MainPageConstants.ImpactStatistic.MaxCount)
            .WithMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(UpdateMainPageDto.ImpactStatistics), MainPageConstants.ImpactStatistic.MaxCount));

        RuleForEach(x => x.ImpactStatistics!)
            .NotNull()
            .SetValidator(new UpdateImpactStatisticDtoValidator());

        RuleFor(x => x.ImpactStatistics)
            .Must(stats =>
            {
                if (stats is null)
                {
                    return true;
                }

                if (stats.Any(s => s is null))
                {
                    return true;
                }

                var ids = stats
                    .Where(s => s!.Id.HasValue)
                    .Select(s => s!.Id!.Value)
                    .ToList();

                return ids.Distinct().Count() == ids.Count;
            })
            .WithMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(UpdateMainPageDto.ImpactStatistics)));
    }
}