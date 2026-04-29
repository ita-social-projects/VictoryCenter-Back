using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class CreateImpactStatisticLocalizationValidator : AbstractValidator<CreateImpactStatisticLocalizationDto>
{
    public CreateImpactStatisticLocalizationValidator()
    {
        RuleFor(x => x.LanguageId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateImpactStatisticLocalizationDto.LanguageId)));

        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImpactStatisticLocalizationDto.Title)))
            .MinimumLength(MainPageConstants.ImpactStatistic.TitleConstraints.MinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateImpactStatisticLocalizationDto.Title),
                MainPageConstants.ImpactStatistic.TitleConstraints.MinLength))
            .MaximumLength(MainPageConstants.ImpactStatistic.TitleConstraints.MaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateImpactStatisticLocalizationDto.Title),
                MainPageConstants.ImpactStatistic.TitleConstraints.MaxLength));
    }
}
