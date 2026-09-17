using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Queries.Admin.FeedbackHistories.Search;

namespace VictoryCenter.BLL.Validators.FeedbackHistories;

public class SearchFeedbackHistoryValidator : AbstractValidator<SearchFeedbackHistoryQuery>
{
    public SearchFeedbackHistoryValidator()
    {
        RuleFor(x => x.SearchDto.SearchQuery)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchFeedbackHistoryDto.SearchQuery)))
            .MinimumLength(GlobalSearchConstants.DefaultSearchQueryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchFeedbackHistoryDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMinLength))
            .MaximumLength(GlobalSearchConstants.DefaultSearchQueryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchFeedbackHistoryDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}