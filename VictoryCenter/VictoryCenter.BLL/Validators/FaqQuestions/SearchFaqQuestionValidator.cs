using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.BLL.Queries.Admin.FaqQuestions.Search;

namespace VictoryCenter.BLL.Validators.FaqQuestions;

public class SearchFaqQuestionValidator : AbstractValidator<SearchFaqQuestionQuery>
{
    public SearchFaqQuestionValidator()
    {
        RuleFor(x => x.SearchFaqQuestionDto.SearchQuery)
    .NotEmpty()
    .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
        nameof(SearchFaqQuestionDto.SearchQuery)))
    .MinimumLength(GlobalSearchConstants.DefaultSearchQueryMinLength)
    .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
        nameof(SearchFaqQuestionDto.SearchQuery),
        GlobalSearchConstants.DefaultSearchQueryMinLength))
    .MaximumLength(GlobalSearchConstants.DefaultSearchQueryMaxLength)
    .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
        nameof(SearchFaqQuestionDto.SearchQuery),
        GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
