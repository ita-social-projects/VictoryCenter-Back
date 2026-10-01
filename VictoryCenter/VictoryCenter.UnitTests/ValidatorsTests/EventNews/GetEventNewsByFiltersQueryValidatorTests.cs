using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetByFilters;
using VictoryCenter.BLL.Validators.EventNews;
using Status = VictoryCenter.DAL.Enums.Status;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class GetEventNewsByFiltersQueryValidatorTests
{
    private readonly GetEventNewsByFiltersQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenFilterValuesAreValid_HasNoErrors()
    {
        var query = Query(status: Status.Draft, offset: 0, limit: 20, categoryId: 1);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenFilterValuesAreNull_HasNoErrors()
    {
        var result = _validator.TestValidate(Query());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenOffsetIsNegative_HasExpectedError()
    {
        var result = _validator.TestValidate(Query(offset: -1));

        result.ShouldHaveValidationErrorFor(query => query.Filter.Offset)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN("Offset", 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenLimitIsNotPositive_HasExpectedError(int limit)
    {
        var result = _validator.TestValidate(Query(limit: limit));

        result.ShouldHaveValidationErrorFor(query => query.Filter.Limit)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThan("Limit", 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenCategoryIdIsNotPositive_HasExpectedError(long categoryId)
    {
        var result = _validator.TestValidate(Query(categoryId: categoryId));

        result.ShouldHaveValidationErrorFor(query => query.Filter.CategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive("CategoryId"));
    }

    [Fact]
    public void Validate_WhenStatusIsInvalid_HasExpectedError()
    {
        // Arrange
        var invalidStatus = Enum.GetValues<Status>().Max() + 1;

        // Act
        var result = _validator.TestValidate(Query(status: invalidStatus));

        // Assert
        result.ShouldHaveValidationErrorFor(query => query.Filter.Status)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum("Status"));
    }

    private static GetEventNewsByFiltersQuery Query(
        Status? status = null,
        int? offset = null,
        int? limit = null,
        long? categoryId = null)
    {
        return new GetEventNewsByFiltersQuery(
            new EventNewsFilterDto
            {
                Status = status,
                Offset = offset,
                Limit = limit,
                CategoryId = categoryId
            });
    }
}
