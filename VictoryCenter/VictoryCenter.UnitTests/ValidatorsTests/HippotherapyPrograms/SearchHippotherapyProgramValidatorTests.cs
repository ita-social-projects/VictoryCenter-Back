using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.BLL.Queries.Admin.HippotherapyPrograms.Search;
using VictoryCenter.BLL.Validators.HippotherapyPrograms;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyPrograms;

public class SearchHippotherapyProgramValidatorTests
{
    private readonly SearchHippotherapyProgramValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveErrors()
    {
        var dto = new SearchHippotherapyProgramDto
        {
            SearchQuery = "Test",
        };
        var query = new SearchHippotherapyProgramsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_InvalidQuery_SearchQueryEmptyShouldHaveError(string? searchQuery)
    {
        var dto = new SearchHippotherapyProgramDto
        {
            SearchQuery = searchQuery!,
        };
        var query = new SearchHippotherapyProgramsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SearchHippotherapyProgramDto.SearchQuery)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchHippotherapyProgramDto.SearchQuery)));
    }

    [Fact]
    public void Validate_InvalidQuery_SearchQueryTooShortShouldHaveError()
    {
        var dto = new SearchHippotherapyProgramDto
        {
            SearchQuery = "A",
        };
        var query = new SearchHippotherapyProgramsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SearchHippotherapyProgramDto.SearchQuery)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchHippotherapyProgramDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMinLength));
    }

    [Fact]
    public void Validate_InvalidQuery_SearchQueryTooLongShouldHaveError()
    {
        string searchQuery = new('A', GlobalSearchConstants.DefaultSearchQueryMaxLength + 1);
        var dto = new SearchHippotherapyProgramDto
        {
            SearchQuery = searchQuery,
        };
        var query = new SearchHippotherapyProgramsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SearchHippotherapyProgramDto.SearchQuery)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchHippotherapyProgramDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
