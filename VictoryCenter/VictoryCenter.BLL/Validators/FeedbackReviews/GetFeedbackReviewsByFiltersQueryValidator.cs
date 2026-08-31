using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetByFilters;

namespace VictoryCenter.BLL.Validators.FeedbackReviews;

public class GetFeedbackReviewsByFiltersQueryValidator : AbstractValidator<GetFeedbackReviewsByFiltersQuery>
{
    public GetFeedbackReviewsByFiltersQueryValidator()
    {
        RuleFor(query => query.Filter.Offset)
            .GreaterThanOrEqualTo(0)
            .When(query => query.Filter.Offset.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN("Offset", 0));

        RuleFor(query => query.Filter.Limit)
            .GreaterThan(0)
            .When(query => query.Filter.Limit.HasValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan("Limit", 0));
    }
}
