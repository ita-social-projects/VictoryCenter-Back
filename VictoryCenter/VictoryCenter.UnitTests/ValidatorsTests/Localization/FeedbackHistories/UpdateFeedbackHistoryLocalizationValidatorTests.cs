using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.FeedbackHistories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.FeedbackHistories;
using VictoryCenter.BLL.Validators.Localization.FeedbackHistories;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.FeedbackHistories;

public class UpdateFeedbackHistoryLocalizationValidatorTests
{
    private readonly UpdateFeedbackHistoryLocalizationValidator _validator = new();

    private static UpdateFeedbackHistoryLocalizationDto ValidDto => new()
    {
        Title = "Recovery journey",
        Story = "A sufficiently long English translation of the success story."
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long invalidEntityId)
    {
        var command = new UpdateFeedbackHistoryLocalizationCommand(invalidEntityId, 2, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EntityId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = new UpdateFeedbackHistoryLocalizationCommand(1, invalidLanguageId, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LanguageId);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty()
    {
        var command = new UpdateFeedbackHistoryLocalizationCommand(1, 2, ValidDto with { Title = string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackHistoryLocalizationDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleTooLong()
    {
        var command = new UpdateFeedbackHistoryLocalizationCommand(1, 2, ValidDto with
        {
            Title = new string('A', FeedbackHistoryConstants.TitleMaxLength + 1)
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateFeedbackHistoryLocalizationDto.Title),
                FeedbackHistoryConstants.TitleMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenStoryIsEmpty()
    {
        var command = new UpdateFeedbackHistoryLocalizationCommand(1, 2, ValidDto with { Story = string.Empty });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Localization.Story)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateFeedbackHistoryLocalizationDto.Story)));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDtoIsValid()
    {
        var command = new UpdateFeedbackHistoryLocalizationCommand(1, 2, ValidDto);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
