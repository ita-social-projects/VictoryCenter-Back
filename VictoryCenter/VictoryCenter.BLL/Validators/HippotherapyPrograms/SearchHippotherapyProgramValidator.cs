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
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchHippotherapyProgramDto.SearchQuery)))
            .MinimumLength(SearchQueryMinLength).WithMessage(ErrorMessagesConstants
            .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(SearchHippotherapyProgramDto.SearchQuery), SearchQueryMinLength))
            .MaximumLength(SearchQueryMaxLength).WithMessage(ErrorMessagesConstants
            .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(SearchHippotherapyProgramDto.SearchQuery), SearchQueryMaxLength));
    }

    public static int SearchQueryMinLength { get; } = 2;
    public static int SearchQueryMaxLength { get; } = 100;
}
