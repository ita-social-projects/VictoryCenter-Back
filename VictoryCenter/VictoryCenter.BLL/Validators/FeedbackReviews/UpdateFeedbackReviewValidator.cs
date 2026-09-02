using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.FeedbackReviews;

public class UpdateFeedbackReviewValidator : AbstractValidator<UpdateFeedbackReviewCommand>
{
    public UpdateFeedbackReviewValidator(BaseFeedbackReviewValidator baseReviewValidator)
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateFeedbackReviewCommand.Id)));

        RuleFor(command => command.FeedbackReview)
            .NotNull()
            .SetValidator(baseReviewValidator);
    }
}
