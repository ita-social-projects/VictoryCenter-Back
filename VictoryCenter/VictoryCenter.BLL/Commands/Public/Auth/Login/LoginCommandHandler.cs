using System.Security.Claims;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Public.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.BLL.Commands.Public.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AdminUser> _userManager;
    private readonly IValidator<LoginCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<JwtOptions> _jwtOptions;

    public LoginCommandHandler(ITokenService tokenService, UserManager<AdminUser> userManager, IValidator<LoginCommand> validator, IHttpContextAccessor httpContextAccessor, IOptions<JwtOptions> jwtOptions)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var admin = await _userManager.FindByEmailAsync(request.RequestDto.Email);
        if (admin is null)
        {
            return Result.Fail(AuthConstants.AdminWithGivenEmailWasNotFound);
        }

        var result = await _userManager.CheckPasswordAsync(admin, request.RequestDto.Password);
        if (!result)
        {
            return Result.Fail(AuthConstants.IncorrectPassword);
        }

        var accessToken = _tokenService.CreateAccessToken([
            .. await _userManager.GetClaimsAsync(admin),
            new Claim(ClaimTypes.Email, request.RequestDto.Email)
        ]);
        var refreshToken = _tokenService.CreateRefreshToken([new Claim(ClaimTypes.Email, request.RequestDto.Email)]);
        var refreshTokenExppires = DateTime.UtcNow.Add(TimeSpan.FromDays(_jwtOptions.Value.RefreshTokenLifetimeInDays));
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(AuthConstants.RefreshTokenCookieName, refreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenExppires,
            Path = AuthConstants.RefreshTokenCookiePath
        });

        admin.RefreshToken = refreshToken;
        admin.RefreshTokenValidTo = refreshTokenExppires;

        var updateResult = await _userManager.UpdateAsync(admin);

        return !updateResult.Succeeded
            ? Result.Fail(updateResult.Errors.Select(x => x.Description))
            : Result.Ok(new AuthResponseDto(accessToken));
    }
}
