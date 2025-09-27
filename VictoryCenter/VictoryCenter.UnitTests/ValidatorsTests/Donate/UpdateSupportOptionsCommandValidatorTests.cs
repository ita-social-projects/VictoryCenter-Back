using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Validators.Donate;

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
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        var command = new UpdateSupportOptionsCommand(
            new UpdateSupportOptionsDto { Name = name, Value = "Some value" },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateSupportOptionsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldHaveError_WhenValueIsEmpty(string? value)
    {
        var command = new UpdateSupportOptionsCommand(
            new UpdateSupportOptionsDto { Name = "Option", Value = value },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateSupportOptionsDto.Value)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new UpdateSupportOptionsCommand(
            new UpdateSupportOptionsDto { Name = "Option", Value = "Some value" },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
