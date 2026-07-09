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
            .MinimumLength(MainPageConstants.Metric.Name.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateMetricLocalizationDto.Name), MainPageConstants.Metric.Name.MinLength))
            .MaximumLength(MainPageConstants.Metric.Name.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateMetricLocalizationDto.Name), MainPageConstants.Metric.Name.MaxLength))
            .When(x => x.Name != null);
    }
}
