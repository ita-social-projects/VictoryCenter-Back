using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class UpdateMetricDtoValidator : BaseMetricDtoValidator<UpdateMetricDto>
{
    public UpdateMetricDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .When(x => x.Id.HasValue)
                .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateMetricDto.Id)));
        }
}