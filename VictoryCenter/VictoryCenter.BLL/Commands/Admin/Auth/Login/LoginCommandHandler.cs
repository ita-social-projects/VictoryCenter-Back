using System.Security.Claims;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private static readonly AdminUser DummyAdmin = new();
    private static readonly PasswordHasher<AdminUser> DummyPasswordHasher = new();
    private static readonly string DummyPasswordHash = DummyPasswordHasher.HashPassword(
        DummyAdmin,
        Guid.NewGuid().ToString());

    private readonly ITokenService _tokenService;
    private readonly UserManager<AdminUser> _userManager;
    private readonly IValidator<LoginCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        ITokenService tokenService,
        UserManager<AdminUser> userManager,
        IValidator<LoginCommand> validator,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions,
        ILogger<LoginCommandHandler> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var admin = await _userManager.FindByEmailAsync(request.LoginRequestDto.Email);
        if (admin is null)
        {
            PerformDummyPasswordCheck(request.LoginRequestDto.Password);
            LogFailedLoginAttempt();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var result = await _userManager.CheckPasswordAsync(admin, request.LoginRequestDto.Password);
        if (!result)
        {
            LogFailedLoginAttempt();
            return Result.Fail(AuthConstants.Unauthorized);
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            new Claim(ClaimTypes.Email, request.LoginRequestDto.Email)
        ]);
        var refreshToken = _tokenService.CreateRefreshToken([new Claim(ClaimTypes.Email, request.LoginRequestDto.Email)]);
        var refreshTokenExpires = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays));
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(AuthConstants.RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenExpires,
            Path = AuthConstants.RefreshTokenCookiePath
        });

        admin.RefreshToken = refreshToken;
        admin.RefreshTokenValidTo = refreshTokenExpires;

        var updateResult = await _userManager.UpdateAsync(admin);

        return !updateResult.Succeeded
            ? Result.Fail(updateResult.Errors.Select(x => x.Description))
            : Result.Ok(new AuthResponseDto(accessToken));
    }

    private void LogFailedLoginAttempt()
    {
        _logger.LogWarning(
            "Admin login failed from client IP {ClientIpAddress}",
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress);
    }

    private static void PerformDummyPasswordCheck(string password)
    {
        _ = DummyPasswordHasher.VerifyHashedPassword(DummyAdmin, DummyPasswordHash, password);
    }
}
