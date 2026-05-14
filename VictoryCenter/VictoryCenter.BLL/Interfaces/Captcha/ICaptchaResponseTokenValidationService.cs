using FluentResults;

namespace VictoryCenter.BLL.Interfaces.Captcha;

public interface ICaptchaResponseTokenValidationService
{
    Task<Result> ValidateTokenAsync(string token, string? remoteIp = null);
}
