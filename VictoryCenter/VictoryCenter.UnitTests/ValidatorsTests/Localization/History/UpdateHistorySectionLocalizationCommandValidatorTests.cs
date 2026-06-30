using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;
using VictoryCenter.BLL.Validators.Localization.History;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.History;

public class UpdateHistorySectionLocalizationCommandValidatorTests
{
    private readonly UpdateHistorySectionLocalizationCommandValidator _sut;

    public UpdateHistorySectionLocalizationCommandValidatorTests()
    {
        var contentValidator = new BaseHistorySectionContentLocalizationValidator();
        _sut = new UpdateHistorySectionLocalizationCommandValidator(contentValidator);
    }

    [Fact]
    public void Validate_UpdateDtoIsNull_ShouldHaveValidationError()
    {
        var command = new UpdateHistorySectionLocalizationCommand(null!, 1);

        var result = _sut.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateDto)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHistorySectionLocalizationCommand.UpdateDto)));
    }

    [Fact]
    public void Validate_ContentsIsNull_ShouldHaveValidationError()
    {
        var dto = new UpdateHistorySectionLocalizationDto { Contents = null! };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        var result = _sut.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateDto.Contents)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHistorySectionLocalizationDto.Contents)));
    }

    [Fact]
    public void Validate_ContentsIsEmpty_ShouldHaveValidationError()
    {
        var dto = new UpdateHistorySectionLocalizationDto { Contents = [] };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        var result = _sut.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateDto.Contents)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(nameof(UpdateHistorySectionLocalizationDto.Contents)));
    }

    [Fact]
    public void Validate_ContentsContainsNullElements_ShouldHaveValidationError()
    {
        var dto = new UpdateHistorySectionLocalizationDto { Contents = [null!] };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        var result = _sut.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UpdateDto.Contents)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(UpdateHistorySectionLocalizationDto.Contents)));
    }

    [Fact]
    public void Validate_ContentsAreValid_ShouldNotHaveAnyValidationErrors()
    {
        var dto = new UpdateHistorySectionLocalizationDto
        {
            Contents = [new UpdateHistorySectionContentLocalizationDto { EntityId = 1, Title = "Valid string" }]
        };
        var command = new UpdateHistorySectionLocalizationCommand(dto, 1);

        var result = _sut.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
