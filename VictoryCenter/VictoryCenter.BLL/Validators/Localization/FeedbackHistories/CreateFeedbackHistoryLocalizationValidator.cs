using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;

namespace VictoryCenter.BLL.Validators.Localization.FeedbackHistories;

public class CreateFeedbackHistoryLocalizationValidator
    : AbstractValidator<CreateFeedbackHistoryLocalizationCommand>
{
    public CreateFeedbackHistoryLocalizationValidator()
    {
        RuleFor(command => command.Localization.EntityId).GreaterThan(0);
        RuleFor(command => command.Localization.LanguageId).GreaterThan(0);

        RuleFor(command => command.Localization.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackHistoryLocalizationDto.Title)))
            .MinimumLength(FeedbackHistoryConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryLocalizationDto.Title),
                FeedbackHistoryConstants.TitleMinLength))
            .MaximumLength(FeedbackHistoryConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryLocalizationDto.Title),
                FeedbackHistoryConstants.TitleMaxLength));

        RuleFor(command => command.Localization.Story)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackHistoryLocalizationDto.Story)))
            .MinimumLength(FeedbackHistoryConstants.StoryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryLocalizationDto.Story),
                FeedbackHistoryConstants.StoryMinLength))
            .MaximumLength(FeedbackHistoryConstants.StoryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryLocalizationDto.Story),
                FeedbackHistoryConstants.StoryMaxLength));
    }
}
