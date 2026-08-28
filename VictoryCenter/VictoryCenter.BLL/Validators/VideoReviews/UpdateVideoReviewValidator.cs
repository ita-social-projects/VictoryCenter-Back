using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.VideoReviews.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.VideoReviews;

public class UpdateVideoReviewValidator : AbstractValidator<UpdateVideoReviewCommand>
{
    public UpdateVideoReviewValidator(BaseVideoReviewDtoValidator baseVideoReviewDtoValidator)
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateVideoReviewCommand.Id)));

        RuleFor(command => command.VideoReview)
            .NotNull()
            .SetValidator(baseVideoReviewDtoValidator);
    }
}
