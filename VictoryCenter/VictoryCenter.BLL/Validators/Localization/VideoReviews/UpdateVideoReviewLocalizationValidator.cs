using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

namespace VictoryCenter.BLL.Validators.Localization.VideoReviews;

public class UpdateVideoReviewLocalizationValidator
    : AbstractValidator<UpdateVideoReviewLocalizationCommand>
{
    public UpdateVideoReviewLocalizationValidator()
    {
        RuleFor(command => command.EntityId).GreaterThan(0);
        RuleFor(command => command.LanguageId).GreaterThan(0);

        RuleFor(command => command.Localization.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateVideoReviewLocalizationDto.Title)))
            .Must(title => string.IsNullOrWhiteSpace(title)
                || title.Trim().Length >= VideoReviewConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateVideoReviewLocalizationDto.Title),
                VideoReviewConstants.TitleMinLength))
            .Must(title => string.IsNullOrWhiteSpace(title)
                || title.Trim().Length <= VideoReviewConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateVideoReviewLocalizationDto.Title),
                VideoReviewConstants.TitleMaxLength));
    }
}
