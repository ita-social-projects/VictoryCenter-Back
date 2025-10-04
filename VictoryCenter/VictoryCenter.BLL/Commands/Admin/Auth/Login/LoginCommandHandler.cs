using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Commands.Base;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Constants;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Admin.Auth.Login;

public class LoginCommandHandler : BaseHandler<LoginCommand, AuthResponseDto>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AdminUser> _userManager;
    private readonly IValidator<LoginCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtOptions> _jwtOptions;

    public LoginCommandHandler(
        ITokenService tokenService,
        UserManager<AdminUser> userManager,
        IValidator<LoginCommand> validator,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions;
    }

    public override async Task<AuthResponseDto> HandleRequest(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new Exception(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var admin = await _userManager.FindByEmailAsync(request.LoginRequestDto.Email);
        if (admin is null)
        {
            throw new Exception(AuthConstants.AdminWithGivenEmailWasNotFound);
        }

        var result = await _userManager.CheckPasswordAsync(admin, request.LoginRequestDto.Password);
        if (!result)
        {
            throw new Exception(AuthConstants.IncorrectPassword);
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
            ? throw new Exception(string.Join("; ", updateResult.Errors.Select(x => x.Description)))
            : new AuthResponseDto(accessToken);
    }
}
