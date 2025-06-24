namespace VictoryCenter.BLL.DTOs.Auth;

public record RefreshTokenRequest(string ExpiredAccessToken, string RefreshToken);
