using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Validators.FeedbackHistories;

namespace VictoryCenter.UnitTests.ValidatorsTests.FeedbackHistories;

public class BaseFeedbackHistoryValidatorTests
{
    private readonly BaseFeedbackHistoryValidator _validator;

    public BaseFeedbackHistoryValidatorTests()
    {
        _validator = new BaseFeedbackHistoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BaseFeedbackHistoryValidator_ShouldHaveError_WhenTitleIsEmpty(string? title)
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = title!,
            Story = "Valid story text that meets the length requirements."
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFeedbackHistoryDto.Title)));
    }

    [Fact]
    public void BaseFeedbackHistoryValidator_ShouldHaveError_WhenTitleIsTooShort()
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = new string('A', FeedbackHistoryConstants.TitleMinLength - 1),
            Story = "Valid story text that meets the length requirements."
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryDto.Title),
                FeedbackHistoryConstants.TitleMinLength));
    }

    [Fact]
    public void BaseFeedbackHistoryValidator_ShouldHaveError_WhenTitleIsTooLong()
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = new string('A', FeedbackHistoryConstants.TitleMaxLength + 1),
            Story = "Valid story text that meets the length requirements."
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryDto.Title),
                FeedbackHistoryConstants.TitleMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BaseFeedbackHistoryValidator_ShouldHaveError_WhenStoryIsEmpty(string? story)
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = "Valid Title Long Enough",
            Story = story!
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Story)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateFeedbackHistoryDto.Story)));
    }

    [Fact]
    public void BaseFeedbackHistoryValidator_ShouldHaveError_WhenStoryIsTooLong()
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = "Valid Title Long Enough",
            Story = new string('A', FeedbackHistoryConstants.StoryMaxLength + 1)
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Story)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryDto.Story),
                FeedbackHistoryConstants.StoryMaxLength));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void BaseFeedbackHistoryValidator_ShouldHaveError_WhenImageIdIsNotPositive(long invalidImageId)
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = "Valid Title Long Enough",
            Story = "Valid story text.",
            ImageId = invalidImageId
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ImageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateFeedbackHistoryDto.ImageId)));
    }

    [Fact]
    public void BaseFeedbackHistoryValidator_ShouldNotHaveErrors_WhenImageIdIsNull()
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = "Valid Title Long Enough",
            Story = "Valid story text.",
            ImageId = null
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ImageId);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void BaseFeedbackHistoryValidator_ShouldNotHaveErrors_WhenModelIsValid()
    {
        var model = new CreateFeedbackHistoryDto
        {
            Title = "Valid Title Long Enough",
            Story = "Valid story text.",
            ImageId = 10L
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}