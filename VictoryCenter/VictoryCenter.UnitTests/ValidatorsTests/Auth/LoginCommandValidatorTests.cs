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
    public void Should_Have_Error_When_Email_Is_Empty(string? email)
    {
        var command = new LoginCommand(new LoginRequestDto(email, "password"));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Email)
            .WithErrorMessage("Email cannot be empty");
    }

    [Theory]
    [InlineData("not-an-email")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        var command = new LoginCommand(new LoginRequestDto(email, "password"));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Email)
            .WithErrorMessage("Email address must be in a valid format");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Password_Is_Empty(string? password)
    {
        var command = new LoginCommand(new LoginRequestDto("user@email.com", password));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Password)
            .WithErrorMessage("Password cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Input()
    {
        var command = new LoginCommand(new LoginRequestDto("user@email.com", "password123"));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Password);
    }
}
