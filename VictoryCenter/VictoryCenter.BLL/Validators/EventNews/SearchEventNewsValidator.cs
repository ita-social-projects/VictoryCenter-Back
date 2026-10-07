using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Queries.Admin.EventNews.Search;

namespace VictoryCenter.BLL.Validators.EventNews;

public class SearchEventNewsValidator : AbstractValidator<SearchEventNewsQuery>
{
    public SearchEventNewsValidator()
    {
        RuleFor(x => x.SearchEventNewsDto.SearchQuery)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(SearchEventNewsDto.SearchQuery)))
            .MinimumLength(GlobalSearchConstants.DefaultSearchQueryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchEventNewsDto.SearchQuery),
                GlobalSearchConstants.DefaultSearchQueryMinLength))
            .MaximumLength(GlobalSearchConstants.DefaultSearchQueryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchEventNewsDto.SearchQuery),
                GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
