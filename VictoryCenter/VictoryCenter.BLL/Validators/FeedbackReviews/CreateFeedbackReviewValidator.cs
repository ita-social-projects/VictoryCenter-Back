using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.FeedbackReviews.Create;

namespace VictoryCenter.BLL.Validators.FeedbackReviews;

public class CreateFeedbackReviewValidator : AbstractValidator<CreateFeedbackReviewCommand>
{
    public CreateFeedbackReviewValidator(BaseFeedbackReviewValidator baseReviewValidator)
    {
        RuleFor(command => command.CreateFeedbackReviewDto)
            .NotNull()
            .SetValidator(baseReviewValidator);
    }
}
