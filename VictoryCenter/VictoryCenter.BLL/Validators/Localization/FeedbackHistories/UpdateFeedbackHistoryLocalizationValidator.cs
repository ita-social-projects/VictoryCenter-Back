using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

namespace VictoryCenter.BLL.Validators.Localization.FeedbackHistories;

public class UpdateFeedbackHistoryLocalizationValidator
    : AbstractValidator<UpdateFeedbackHistoryLocalizationCommand>
{
    public UpdateFeedbackHistoryLocalizationValidator()
    {
        RuleFor(command => command.EntityId).GreaterThan(0);
        RuleFor(command => command.LanguageId).GreaterThan(0);

        RuleFor(command => command.Localization.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackHistoryLocalizationDto.Title)))
            .MinimumLength(FeedbackHistoryConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateFeedbackHistoryLocalizationDto.Title),
                FeedbackHistoryConstants.TitleMinLength))
            .MaximumLength(FeedbackHistoryConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateFeedbackHistoryLocalizationDto.Title),
                FeedbackHistoryConstants.TitleMaxLength));

        RuleFor(command => command.Localization.Story)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackHistoryLocalizationDto.Story)))
            .MinimumLength(FeedbackHistoryConstants.StoryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateFeedbackHistoryLocalizationDto.Story),
                FeedbackHistoryConstants.StoryMinLength))
            .MaximumLength(FeedbackHistoryConstants.StoryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateFeedbackHistoryLocalizationDto.Story),
                FeedbackHistoryConstants.StoryMaxLength));
    }
}
