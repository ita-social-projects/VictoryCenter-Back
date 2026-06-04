using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.BLL.Validators.Localization.History;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.History;

public class CreateHistorySectionLocalizationValidatorTests
{
    private readonly CreateHistorySectionLocalizationValidator _validator;
    private readonly Mock<BaseHistorySectionContentLocalizationValidator> _contentValidatorMock;

    public CreateHistorySectionLocalizationValidatorTests()
    {
        _contentValidatorMock = new Mock<BaseHistorySectionContentLocalizationValidator>();
        _validator = new CreateHistorySectionLocalizationValidator(_contentValidatorMock.Object);
    }

    private static CreateHistorySectionContentLocalizationDto ValidContent => new()
    {
        EntityId = 1,
        LanguageId = 1,
        Title = "Valid Title"
    };

    private static CreateHistorySectionLocalizationDto ValidSection => new()
    {
        EntityId = 1,
        Contents = [ValidContent]
    };

    // CreateHistorySectionLocalizationDtos — null
    [Fact]
    public void Validate_WhenDtosIsNull_ShouldHaveValidationError()
    {
        var command = new CreateHistoryLocalizationCommand(null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHistorySectionLocalizationDtos)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                  nameof(CreateHistoryLocalizationCommand.CreateHistorySectionLocalizationDtos)));
    }

    // CreateHistorySectionLocalizationDtos — empty list
    [Fact]
    public void Validate_WhenDtosIsEmpty_ShouldHaveValidationError()
    {
        var command = new CreateHistoryLocalizationCommand([]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHistorySectionLocalizationDtos)
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                  nameof(CreateHistoryLocalizationCommand.CreateHistorySectionLocalizationDtos)));
    }

    // CreateHistorySectionLocalizationDtos — contains null element
    [Fact]
    public void Validate_WhenDtosContainsNullElement_ShouldHaveValidationError()
    {
        var command = new CreateHistoryLocalizationCommand([null!]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CreateHistorySectionLocalizationDtos)
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                  nameof(CreateHistoryLocalizationCommand.CreateHistorySectionLocalizationDtos)));
    }

    // Section.Contents — null
    [Fact]
    public void Validate_WhenSectionContentsIsNull_ShouldHaveValidationError()
    {
        var section = new CreateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = null!
        };
        var command = new CreateHistoryLocalizationCommand([section]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateHistorySectionLocalizationDtos[0].Contents")
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                  nameof(CreateHistorySectionLocalizationDto.Contents)));
    }

    // Section.Contents — empty list
    [Fact]
    public void Validate_WhenSectionContentsIsEmpty_ShouldHaveValidationError()
    {
        var section = new CreateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = []
        };
        var command = new CreateHistoryLocalizationCommand([section]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateHistorySectionLocalizationDtos[0].Contents")
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                  nameof(CreateHistorySectionLocalizationDto.Contents)));
    }

    // Section.Contents — contains null element
    [Fact]
    public void Validate_WhenSectionContentsContainsNullElement_ShouldHaveValidationError()
    {
        var section = new CreateHistorySectionLocalizationDto
        {
            EntityId = 1,
            Contents = [null!]
        };
        var command = new CreateHistoryLocalizationCommand([section]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateHistorySectionLocalizationDtos[0].Contents")
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(
                  nameof(CreateHistorySectionLocalizationDto.Contents)));
    }

    // Valid command — no errors
    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        var command = new CreateHistoryLocalizationCommand([ValidSection]);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CreateHistorySectionLocalizationDtos);
        result.ShouldNotHaveValidationErrorFor("CreateHistorySectionLocalizationDtos[0].Contents");
    }
}
