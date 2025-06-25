using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Auth.RefreshToken;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.BLL.Validators.Auth;

namespace VictoryCenter.UnitTests.ValidatorsTests.Auth;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator;

    public RefreshTokenCommandValidatorTests()
    {
        _validator = new RefreshTokenCommandValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_ExpiredAccessToken_Is_Empty(string? token)
    {
        var command = new RefreshTokenCommand(new RefreshTokenRequest(token, "refresh_token"));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Request.ExpiredAccessToken)
            .WithErrorMessage("Expired access token cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_RefreshToken_Is_Empty(string? refreshToken)
    {
        var command = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", refreshToken));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Request.RefreshToken)
            .WithErrorMessage("Refresh token cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Input()
    {
        var command = new RefreshTokenCommand(new RefreshTokenRequest("expired_access_token", "refresh_token"));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Request.ExpiredAccessToken);
        result.ShouldNotHaveValidationErrorFor(x => x.Request.RefreshToken);
    }
}
