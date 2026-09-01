using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;

namespace VictoryCenter.BLL.Validators.FeedbackHistories;

public class BaseFeedbackHistoryValidator : AbstractValidator<CreateFeedbackHistoryDto>
{
    public BaseFeedbackHistoryValidator()
    {
        RuleFor(dto => dto.Title)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackHistoryDto.Title)))
            .MinimumLength(FeedbackHistoryConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryDto.Title),
                FeedbackHistoryConstants.TitleMinLength))
            .MaximumLength(FeedbackHistoryConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryDto.Title),
                FeedbackHistoryConstants.TitleMaxLength));

        RuleFor(dto => dto.Story)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackHistoryDto.Story)))
            .MinimumLength(FeedbackHistoryConstants.StoryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryDto.Story),
                FeedbackHistoryConstants.StoryMinLength))
            .MaximumLength(FeedbackHistoryConstants.StoryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryDto.Story),
                FeedbackHistoryConstants.StoryMaxLength));

        RuleFor(dto => dto.ImageId)
            .GreaterThan(0)
            .When(dto => dto.ImageId.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateFeedbackHistoryDto.ImageId)));

        RuleFor(dto => dto.Status)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackHistoryDto.Status)));
    }
}
