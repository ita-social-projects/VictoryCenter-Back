using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Validators.Donate;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class CreateSupportOptionsCommandValidatorTests
{
    private readonly CreateSupportOptionsCommandValidator _validator;

    public CreateSupportOptionsCommandValidatorTests()
    {
        _validator = new CreateSupportOptionsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        var command = new CreateSupportOptionsCommand(
            new CreateSupportOptionsDto { Name = name, Value = "SomeValue" });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateSupportOptionsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenValueIsEmpty(string? value)
    {
        var command = new CreateSupportOptionsCommand(
            new CreateSupportOptionsDto { Name = "SomeName", Value = value });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateSupportOptionsDto.Value)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new CreateSupportOptionsCommand(
            new CreateSupportOptionsDto { Name = "SupportName", Value = "SupportValue" });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
