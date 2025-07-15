namespace VictoryCenter.BLL.Constants;

public static class AuthConstants
{
    public const string RefreshTokenCookieName = "refreshToken";
    public const string RefreshTokenCookiePath = "/api/auth/refresh-token";
    public const string AdminWithGivenEmailWasNotFound = "Admin with given email was not found";
    public const string IncorrectPassword = "Incorrect password";
    public const string RefreshTokenIsNotPresent = "Refresh token is not present";
    public const string InvalidToken = "Invalid Token";
    public const string InvalidTokenSignature = "Invalid Token signature";
    public const string RefreshTokenIsInvalid = "Refresh token is invalid";
    public const string RefreshTokenCannotBeNullOrEmpty = "Refresh token cannot be null or empty";
}
