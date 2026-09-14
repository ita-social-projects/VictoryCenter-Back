using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

namespace VictoryCenter.BLL.Validators.Localization.VideoReviews;

public class CreateVideoReviewLocalizationValidator
    : AbstractValidator<CreateVideoReviewLocalizationCommand>
{
    public CreateVideoReviewLocalizationValidator()
    {
        RuleFor(command => command.Localization.EntityId).GreaterThan(0);
        RuleFor(command => command.Localization.LanguageId).GreaterThan(0);

        RuleFor(command => command.Localization.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateVideoReviewLocalizationDto.Title)))
            .Must(title => string.IsNullOrWhiteSpace(title)
                || title.Trim().Length >= VideoReviewConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateVideoReviewLocalizationDto.Title),
                VideoReviewConstants.TitleMinLength))
            .Must(title => string.IsNullOrWhiteSpace(title)
                || title.Trim().Length <= VideoReviewConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateVideoReviewLocalizationDto.Title),
                VideoReviewConstants.TitleMaxLength));
    }
}
