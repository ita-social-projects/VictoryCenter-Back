using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;
using VictoryCenter.BLL.Validators.Localization.History;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.History;

public class UpdateHistorySectionLocalizationValidatorTests
{
    private readonly UpdateHistorySectionLocalizationValidator _validator;
    private readonly Mock<BaseHistorySectionContentLocalizationValidator> _contentValidatorMock;

    public UpdateHistorySectionLocalizationValidatorTests()
    {
        _contentValidatorMock = new Mock<BaseHistorySectionContentLocalizationValidator>();
        _validator = new UpdateHistorySectionLocalizationValidator(_contentValidatorMock.Object);
    }

    private static UpdateHistorySectionContentLocalizationDto ValidContent => new()
    {
        EntityId = 1,
        Title = "Valid Title"
    };

    private static UpdateHistorySectionLocalizationDto ValidSection => new()
    {
        EntityId = 1,
        Contents = [ValidContent]
    };

    // UpdateHistorySectionLocalizationDtos — null
    [Fact]
    public void Validate_WhenDtosIsNull_ShouldHaveValidationError()
    {
        var command = new UpdateHistoryLocalizationCommand(null!, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateHistorySectionLocalizationDtos)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                  nameof(UpdateHistoryLocalizationCommand.UpdateHistorySectionLocalizationDtos)));
    }

    // UpdateHistorySectionLocalizationDtos — empty list
    [Fact]
    public void Validate_WhenDtosIsEmpty_ShouldHaveValidationError()
    {
        var command = new UpdateHistoryLocalizationCommand([], 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateHistorySectionLocalizationDtos)
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                  nameof(UpdateHistoryLocalizationCommand.UpdateHistorySectionLocalizationDtos)));
    }

    // UpdateHistorySectionLocalizationDtos — contains null element
    [Fact]
    public void Validate_WhenDtosContainsNullElement_ShouldHaveValidationError()
    {
        var command = new UpdateHistoryLocalizationCommand([null!], 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateHistorySectionLocalizationDtos)
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                  nameof(UpdateHistoryLocalizationCommand.UpdateHistorySectionLocalizationDtos)));
    }

    // Section.Contents — null
    [Fact]
    public void Validate_WhenSectionContentsIsNull_ShouldHaveValidationError()
    {
        var section = new UpdateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = null!
        };
        var command = new UpdateHistoryLocalizationCommand([section], 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("UpdateHistorySectionLocalizationDtos[0].Contents")
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                  nameof(UpdateHistorySectionLocalizationDto.Contents)));
    }

    // Section.Contents — empty list
    [Fact]
    public void Validate_WhenSectionContentsIsEmpty_ShouldHaveValidationError()
    {
        var section = new UpdateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = []
        };
        var command = new UpdateHistoryLocalizationCommand([section], 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("UpdateHistorySectionLocalizationDtos[0].Contents")
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                  nameof(UpdateHistorySectionLocalizationDto.Contents)));
    }

    // Section.Contents — contains null element
    [Fact]
    public void Validate_WhenSectionContentsContainsNullElement_ShouldHaveValidationError()
    {
        var section = new UpdateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = [null!]
        };
        var command = new UpdateHistoryLocalizationCommand([section], 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("UpdateHistorySectionLocalizationDtos[0].Contents")
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                  nameof(UpdateHistorySectionLocalizationDto.Contents)));
    }

    // Valid command — no errors
    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        var command = new UpdateHistoryLocalizationCommand([ValidSection], 1);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.UpdateHistorySectionLocalizationDtos);
        result.ShouldNotHaveValidationErrorFor("UpdateHistorySectionLocalizationDtos[0].Contents");
    }
}
