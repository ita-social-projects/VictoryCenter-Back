using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Validators.Donate.SupportOptions;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class UpdateSupportOptionsCommandValidatorTests
{
    private readonly UpdateSupportOptionsCommandValidator _validator;

    public UpdateSupportOptionsCommandValidatorTests()
    {
        _validator = new UpdateSupportOptionsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new UpdateSupportOptionsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateSupportOptionsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenValueIsEmpty(string? value)
    {
        // Arrange
        var dto = GetValidDto() with { Value = value! };
        var command = new UpdateSupportOptionsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateSupportOptionsDto.Value)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();
        var command = new UpdateSupportOptionsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateSupportOptionsDto GetValidDto() => new()
    {
        Name = "Valid Name",
        Value = "Valid Value"
    };
}
