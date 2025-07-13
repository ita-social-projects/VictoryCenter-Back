using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Auth.Login;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Validators.Auth;

namespace VictoryCenter.UnitTests.ValidatorsTests.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmailIsEmpty_ShouldHaveValidationError(string? email)
    {
        var command = new LoginCommand(new LoginRequestDto(email, "password"));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Email)
            .WithErrorMessage("Email cannot be empty");
    }

    [Theory]
    [InlineData("not-an-email")]
    public void Validate_EmailIsInvalid_ShouldHaveValidationError(string email)
    {
        var command = new LoginCommand(new LoginRequestDto(email, "password"));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Email)
            .WithErrorMessage("Email address must be in a valid format");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_PasswordIsEmpty_ShouldHaveValidationError(string? password)
    {
        var command = new LoginCommand(new LoginRequestDto("user@email.com", password));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Password)
            .WithErrorMessage("Password cannot be empty");
    }

    [Fact]
    public void Validate_InputIsValid_ShouldNotHaveValidationErrors()
    {
        var command = new LoginCommand(new LoginRequestDto("user@email.com", "password123"));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Password);
    }
}
