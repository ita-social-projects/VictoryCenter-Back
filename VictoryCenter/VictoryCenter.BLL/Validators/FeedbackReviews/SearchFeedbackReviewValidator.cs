using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Queries.Admin.FeedbackReviews.Search;

namespace VictoryCenter.BLL.Validators.FeedbackReviews;

public class SearchFeedbackReviewValidator : AbstractValidator<SearchFeedbackReviewQuery>
{
    public SearchFeedbackReviewValidator()
    {
        RuleFor(x => x.SearchDto.SearchQuery)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchFeedbackReviewDto.SearchQuery)))
            .MinimumLength(GlobalSearchConstants.DefaultSearchQueryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchFeedbackReviewDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMinLength))
            .MaximumLength(GlobalSearchConstants.DefaultSearchQueryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchFeedbackReviewDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
