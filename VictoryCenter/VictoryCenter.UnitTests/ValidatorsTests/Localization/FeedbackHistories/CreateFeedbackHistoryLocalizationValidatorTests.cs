using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.BLL.Validators.Localization.FeedbackHistories;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.FeedbackHistories;

public class CreateFeedbackHistoryLocalizationValidatorTests
{
    private readonly CreateFeedbackHistoryLocalizationValidator _validator = new();

    private static CreateFeedbackHistoryLocalizationDto ValidDto => new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Recovery journey",
        Story = "A sufficiently long English translation of the success story."
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long invalidEntityId)
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto with { EntityId = invalidEntityId });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.EntityId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto with { LanguageId = invalidLanguageId });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.LanguageId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string invalidTitle)
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto with { Title = invalidTitle });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackHistoryLocalizationDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleTooShort()
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto with { Title = "Short" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryLocalizationDto.Title),
                FeedbackHistoryConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleTooLong()
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto with
        {
            Title = new string('A', FeedbackHistoryConstants.TitleMaxLength + 1)
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryLocalizationDto.Title),
                FeedbackHistoryConstants.TitleMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenStoryIsEmpty()
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto with { Story = string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Story)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateFeedbackHistoryLocalizationDto.Story)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenStoryTooLong()
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto with
        {
            Story = new string('A', FeedbackHistoryConstants.StoryMaxLength + 1)
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Story)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateFeedbackHistoryLocalizationDto.Story),
                FeedbackHistoryConstants.StoryMaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new CreateFeedbackHistoryLocalizationCommand(ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
