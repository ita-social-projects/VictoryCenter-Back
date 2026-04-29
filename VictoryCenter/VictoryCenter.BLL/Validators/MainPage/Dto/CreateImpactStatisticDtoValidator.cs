using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.Validators.Localization.MainPage;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class CreateImpactStatisticDtoValidator
    : BaseImpactStatisticDtoValidator<CreateImpactStatisticDto, CreateMetricDto>
{
    public CreateImpactStatisticDtoValidator()
        : base(dto => dto.Metrics)
    {
        RuleForEach(x => x.Metrics)
            .SetValidator(new CreateMetricDtoValidator())
            .When(x => x.Metrics is not null);

        RuleForEach(x => x.Metrics)
            .Must(m => m is null || m.Type == MetricType.Raised || m.Localization == null)
            .WithMessage(ErrorMessagesConstants.LocalizationOnlyAllowedForRaisedMetric());

        When(x => x.Localization is not null, () =>
        {
            RuleFor(x => x.Localization!)
                .SetValidator(new CreateImpactStatisticLocalizationValidator());
        });
    }
}
