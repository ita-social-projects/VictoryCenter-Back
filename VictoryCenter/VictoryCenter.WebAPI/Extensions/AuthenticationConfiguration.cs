using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services.TokenService;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.WebAPI.Authentication;

namespace VictoryCenter.WebAPI.Extensions;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAdminAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.Position))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddIdentity<AdminUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<VictoryCenterDbContext>()
            .AddDefaultTokenProviders();

        services.AddOptions<AccountLockoutOptions>()
            .Bind(configuration.GetSection(AccountLockoutOptions.Position))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<IdentityOptions>()
            .Configure<IOptions<AccountLockoutOptions>>((identityOptions, lockoutOptions) =>
            {
                identityOptions.Lockout.AllowedForNewUsers = true;
                identityOptions.Lockout.MaxFailedAccessAttempts = lockoutOptions.Value.MaxFailedAccessAttempts;
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutOptions.Value.LockoutMinutes);
            });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearerOptions, jwtOptions) =>
            {
                bearerOptions.TokenValidationParameters = AuthHelper.GetTokenValidationParameters(jwtOptions.Value);
                bearerOptions.EventsType = typeof(AccessTokenValidationEvents);
            });

        // Existing [Authorize] endpoints accept admin bearer tokens, never Identity cookies.
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });
        services.AddScoped<AccessTokenValidationEvents>();
        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }
}
