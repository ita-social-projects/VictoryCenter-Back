using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Validators.FeedbackReviews;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.FeedbackReviews;

public class BaseFeedbackReviewValidatorTests
{
    private readonly BaseFeedbackReviewValidator _validator = new();

    [Fact]
    public void Validator_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var result = _validator.TestValidate(ValidDto());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_ShouldHaveError_WhenAuthorNameIsEmptyOrWhitespace(string? authorName)
    {
        var result = _validator.TestValidate(ValidDto() with { AuthorName = authorName! });

        result.ShouldHaveValidationErrorFor(dto => dto.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackReviewDto.AuthorName)));
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenAuthorNameIsShorterThanMinLengthAfterTrimming()
    {
        var shortName = new string('a', FeedbackReviewConstants.AuthorNameMinLength - 1);
        var result = _validator.TestValidate(ValidDto() with { AuthorName = $"  {shortName}  " });

        result.ShouldHaveValidationErrorFor(dto => dto.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMinLength));
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenAuthorNameIsExactlyAtMinLengthAfterTrimming()
    {
        var name = new string('a', FeedbackReviewConstants.AuthorNameMinLength);
        var result = _validator.TestValidate(ValidDto() with { AuthorName = $"  {name}  " });

        result.ShouldNotHaveValidationErrorFor(dto => dto.AuthorName);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenAuthorNameExceedsMaxLength()
    {
        var longName = new string('a', FeedbackReviewConstants.AuthorNameMaxLength + 1);
        var result = _validator.TestValidate(ValidDto() with { AuthorName = longName });

        result.ShouldHaveValidationErrorFor(dto => dto.AuthorName)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.AuthorName),
                FeedbackReviewConstants.AuthorNameMaxLength));
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenAuthorNameIsExactlyAtMaxLength()
    {
        var name = new string('a', FeedbackReviewConstants.AuthorNameMaxLength);
        var result = _validator.TestValidate(ValidDto() with { AuthorName = name });

        result.ShouldNotHaveValidationErrorFor(dto => dto.AuthorName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_ShouldHaveError_WhenTextIsEmptyOrWhitespace(string? text)
    {
        var result = _validator.TestValidate(ValidDto() with { Text = text! });

        result.ShouldHaveValidationErrorFor(dto => dto.Text)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackReviewDto.Text)));
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenTextIsShorterThanMinLengthAfterTrimming()
    {
        var shortText = new string('a', FeedbackReviewConstants.TextMinLength - 1);
        var result = _validator.TestValidate(ValidDto() with { Text = $"  {shortText}  " });

        result.ShouldHaveValidationErrorFor(dto => dto.Text)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.Text),
                FeedbackReviewConstants.TextMinLength));
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenTextExceedsMaxLength()
    {
        var longText = new string('a', FeedbackReviewConstants.TextMaxLength + 1);
        var result = _validator.TestValidate(ValidDto() with { Text = longText });

        result.ShouldHaveValidationErrorFor(dto => dto.Text)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackReviewDto.Text),
                FeedbackReviewConstants.TextMaxLength));
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenTextIsExactlyAtMaxLength()
    {
        var text = new string('a', FeedbackReviewConstants.TextMaxLength);
        var result = _validator.TestValidate(ValidDto() with { Text = text });

        result.ShouldNotHaveValidationErrorFor(dto => dto.Text);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenStatusIsNotAValidEnumValue()
    {
        var result = _validator.TestValidate(ValidDto() with { Status = (Status)99 });

        result.ShouldHaveValidationErrorFor(dto => dto.Status)
            .WithErrorMessage(ErrorMessagesConstants.UnknownStatusValue);
    }

    private static CreateFeedbackReviewDto ValidDto() => new()
    {
        AuthorName = "Anastasiia",
        Text = "Very happy with the therapy sessions",
        Status = Status.Draft
    };
}
