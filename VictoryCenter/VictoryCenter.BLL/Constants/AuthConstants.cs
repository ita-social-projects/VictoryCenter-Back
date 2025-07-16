namespace VictoryCenter.BLL.Constants;

public static class AuthConstants
{
    public static readonly string RefreshTokenCookieName = "refreshToken";
    public static readonly string RefreshTokenCookiePath = "/api/auth/refresh-token";
    public static readonly string AdminWithGivenEmailWasNotFound = "Admin with given email was not found";
    public static readonly string IncorrectPassword = "Incorrect password";
    public static readonly string RefreshTokenIsNotPresent = "Refresh token is not present";
    public static readonly string InvalidToken = "Invalid Token";
    public static readonly string InvalidTokenSignature = "Invalid Token signature";
    public static readonly string RefreshTokenIsInvalid = "Refresh token is invalid";
    public static readonly string RefreshTokenCannotBeNullOrEmpty = "Refresh token cannot be null or empty";
}
