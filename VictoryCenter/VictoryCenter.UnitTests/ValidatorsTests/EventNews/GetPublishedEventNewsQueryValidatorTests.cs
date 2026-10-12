using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;
using VictoryCenter.BLL.Validators.EventNews;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class GetPublishedEventNewsQueryValidatorTests
{
    private readonly GetPublishedEventNewsQueryValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllParametersAreNull()
    {
        var result = _validator.TestValidate(CreateQuery());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenCategoryIdIsProvided()
    {
        var result = _validator.TestValidate(CreateQuery(categoryId: 1));

        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1000)]
    public void Validate_ShouldNotHaveErrors_WhenOffsetIsNonNegative(int offset)
    {
        var result = _validator.TestValidate(CreateQuery(offset: offset));

        result.ShouldNotHaveValidationErrorFor(x => x.Offset);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_ShouldHaveError_WhenOffsetIsNegative(int offset)
    {
        var result = _validator.TestValidate(CreateQuery(offset: offset));

        result.ShouldHaveValidationErrorFor(x => x.Offset)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Offset),
                0));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenLimitIsLessThanMinimum()
    {
        var result = _validator.TestValidate(
            CreateQuery(limit: EventNewsConstants.PublishedTakeMinValue - 1));

        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Limit),
                EventNewsConstants.PublishedTakeMinValue));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenLimitIsGreaterThanMaximum()
    {
        var result = _validator.TestValidate(
            CreateQuery(limit: EventNewsConstants.PublishedTakeMaxValue + 1));

        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Limit),
                EventNewsConstants.PublishedTakeMaxValue));
    }

    [Theory]
    [InlineData(EventNewsConstants.PublishedTakeMinValue)]
    [InlineData(EventNewsConstants.PublishedTakeMaxValue)]
    public void Validate_ShouldNotHaveErrors_WhenLimitIsOnAllowedBoundary(int limit)
    {
        var result = _validator.TestValidate(CreateQuery(limit: limit));

        result.ShouldNotHaveValidationErrorFor(x => x.Limit);
    }

    private static GetPublishedEventNewsQuery CreateQuery(
       long? categoryId = null,
       int? offset = null,
       int? limit = null) => new(categoryId, offset, limit);
}
