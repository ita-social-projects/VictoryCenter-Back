using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;

namespace VictoryCenter.BLL.Validators.Localization.History;

public class BaseHistorySectionContentLocalizationValidator : AbstractValidator<IHistoryContentLocalization>
{
    public BaseHistorySectionContentLocalizationValidator()
    {
        RuleFor(x => x.Title)
           .MinimumLength(HistoryLocalizationConstants.ContentTitleMinLength)
           .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
               nameof(CreateHistorySectionContentLocalizationDto.Title),
               HistoryLocalizationConstants.ContentTitleMinLength))
           .MaximumLength(HistoryLocalizationConstants.ContentTitleMaxLength)
           .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
               nameof(CreateHistorySectionContentLocalizationDto.Title),
               HistoryLocalizationConstants.ContentTitleMaxLength))
           .When(x => !string.IsNullOrWhiteSpace(x.Title));
        RuleFor(x => x.Description)
            .MinimumLength(HistoryLocalizationConstants.ContentDescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateHistorySectionContentLocalizationDto.Description),
                HistoryLocalizationConstants.ContentDescriptionMinLength))
            .MaximumLength(HistoryLocalizationConstants.ContentDescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateHistorySectionContentLocalizationDto.Description),
                HistoryLocalizationConstants.ContentDescriptionMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
