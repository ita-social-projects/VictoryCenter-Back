using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Queries.Admin.EventNews.Search;
using VictoryCenter.BLL.Validators.EventNews;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class SearchEventNewsValidatorTests
{
    private readonly SearchEventNewsValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveErrors()
    {
        var dto = new SearchEventNewsDto
        {
            SearchQuery = "Festival",
        };
        var query = new SearchEventNewsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_InvalidQuery_SearchQueryEmptyShouldHaveError(string? searchQuery)
    {
        var dto = new SearchEventNewsDto
        {
            SearchQuery = searchQuery!,
        };
        var query = new SearchEventNewsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SearchEventNewsDto.SearchQuery)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SearchEventNewsDto.SearchQuery)));
    }

    [Fact]
    public void Validate_InvalidQuery_SearchQueryTooShortShouldHaveError()
    {
        var dto = new SearchEventNewsDto
        {
            SearchQuery = "A",
        };
        var query = new SearchEventNewsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SearchEventNewsDto.SearchQuery)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(SearchEventNewsDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMinLength));
    }

    [Fact]
    public void Validate_InvalidQuery_SearchQueryTooLongShouldHaveError()
    {
        string searchQuery = new('A', GlobalSearchConstants.DefaultSearchQueryMaxLength + 1);
        var dto = new SearchEventNewsDto
        {
            SearchQuery = searchQuery,
        };
        var query = new SearchEventNewsQuery(dto);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.SearchEventNewsDto.SearchQuery)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(SearchEventNewsDto.SearchQuery), GlobalSearchConstants.DefaultSearchQueryMaxLength));
    }
}
