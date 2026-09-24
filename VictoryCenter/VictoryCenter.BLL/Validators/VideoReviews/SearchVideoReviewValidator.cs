using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Queries.Admin.VideoReviews.Search;

namespace VictoryCenter.BLL.Validators.VideoReviews;

public class SearchVideoReviewValidator : AbstractValidator<SearchVideoReviewQuery>
{
    public SearchVideoReviewValidator()
    {
        RuleFor(x => x.SearchDto.SearchQuery)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchVideoReviewDto.SearchQuery)))
            .MinimumLength(GlobalSearchConstants.DefaultSearchQueryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchVideoReviewDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMinLength))
            .MaximumLength(GlobalSearchConstants.DefaultSearchQueryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchVideoReviewDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}