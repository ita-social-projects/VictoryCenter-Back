using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;
using VictoryCenter.BLL.Validators.EventNews;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class GetPublishedEventNewsQueryValidatorTests
{
    private readonly GetPublishedEventNewsQueryValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenTakeIsNull()
    {
        var result = _validator.TestValidate(new GetPublishedEventNewsQuery());

        result.ShouldNotHaveValidationErrorFor(x => x.Take);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTakeIsLessThanMinimum()
    {
        var result = _validator.TestValidate(
            new GetPublishedEventNewsQuery(EventNewsConstants.PublishedTakeMinValue - 1));

        result.ShouldHaveValidationErrorFor(x => x.Take)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Take),
                EventNewsConstants.PublishedTakeMinValue));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTakeIsGreaterThanMaximum()
    {
        var result = _validator.TestValidate(
            new GetPublishedEventNewsQuery(EventNewsConstants.PublishedTakeMaxValue + 1));

        result.ShouldHaveValidationErrorFor(x => x.Take)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                nameof(GetPublishedEventNewsQuery.Take),
                EventNewsConstants.PublishedTakeMaxValue));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenTakeIsWithinAllowedRange()
    {
        var result = _validator.TestValidate(
            new GetPublishedEventNewsQuery(EventNewsConstants.PublishedTakeMaxValue));

        result.ShouldNotHaveValidationErrorFor(x => x.Take);
    }
}
