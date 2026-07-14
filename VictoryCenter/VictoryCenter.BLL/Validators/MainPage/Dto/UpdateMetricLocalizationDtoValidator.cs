using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

namespace VictoryCenter.BLL.Validators.MainPage.Dto;

public class UpdateMetricLocalizationDtoValidator : AbstractValidator<UpdateMetricLocalizationDto>
{
    public UpdateMetricLocalizationDtoValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateMetricLocalizationDto.Name)))
            .MinimumLength(MainPageConstants.Metric.ValidationNameRules.MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateMetricLocalizationDto.Name), MainPageConstants.Metric.ValidationNameRules.MinLen))
            .MaximumLength(MainPageConstants.Metric.ValidationNameRules.MaxLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateMetricLocalizationDto.Name), MainPageConstants.Metric.ValidationNameRules.MaxLen))
            .When(x => x.Name != null);
    }
}
