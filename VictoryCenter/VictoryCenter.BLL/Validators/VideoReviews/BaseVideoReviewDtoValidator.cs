using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;

namespace VictoryCenter.BLL.Validators.VideoReviews;

public class BaseVideoReviewDtoValidator : AbstractValidator<CreateVideoReviewDto>
{
    public BaseVideoReviewDtoValidator()
    {
        RuleFor(dto => dto.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateVideoReviewDto.Title)))
            .Must(title => string.IsNullOrWhiteSpace(title) || title.Trim().Length >= VideoReviewConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateVideoReviewDto.Title),
                VideoReviewConstants.TitleMinLength))
            .Must(title => string.IsNullOrWhiteSpace(title) || title.Trim().Length <= VideoReviewConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateVideoReviewDto.Title),
                VideoReviewConstants.TitleMaxLength));

        RuleFor(dto => dto.Link)
            .Must(link => !string.IsNullOrWhiteSpace(link))
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateVideoReviewDto.Link)))
            .Must(link => string.IsNullOrWhiteSpace(link) || link.Trim().Length >= VideoReviewConstants.LinkMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateVideoReviewDto.Link),
                VideoReviewConstants.LinkMinLength))
            .Must(link => string.IsNullOrWhiteSpace(link) || link.Trim().Length <= VideoReviewConstants.LinkMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateVideoReviewDto.Link),
                VideoReviewConstants.LinkMaxLength))
            .Must(link => string.IsNullOrWhiteSpace(link) || Uri.TryCreate(link.Trim(), UriKind.Absolute, out _))
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(nameof(CreateVideoReviewDto.Link)));
    }
}
