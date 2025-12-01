using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.Search;

namespace VictoryCenter.BLL.Validators.HippotherapyPrograms;

public class SearchHippotherapyProgramValidator : AbstractValidator<SearchHippotherapyProgramsQuery>
{
    public SearchHippotherapyProgramValidator()
    {
        RuleFor(x => x.SearchHippotherapyProgramDto.SearchQuery)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(SearchHippotherapyProgramDto.SearchQuery)))
            .MinimumLength(GlobalSearchConstants.DefaultSearchQueryMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchHippotherapyProgramDto.SearchQuery),
                GlobalSearchConstants.DefaultSearchQueryMinLength))
            .MaximumLength(GlobalSearchConstants.DefaultSearchQueryMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchHippotherapyProgramDto.SearchQuery),
                GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
