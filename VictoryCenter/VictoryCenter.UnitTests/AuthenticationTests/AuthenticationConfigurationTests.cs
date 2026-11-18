using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VictoryCenter.BLL.Options;
using VictoryCenter.WebAPI.Authentication;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.UnitTests.AuthenticationTests;

public class AuthenticationConfigurationTests
{
    [Fact]
    public void Registration_UsesBearerAuthenticationAndExplicitAuthorizationWithoutFallback()
    {
        using var provider = CreateProvider();
        var authentication = provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        var authorization = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        var bearer = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authentication.DefaultScheme);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authentication.DefaultAuthenticateScheme);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authentication.DefaultChallengeScheme);
        Assert.Equal([JwtBearerDefaults.AuthenticationScheme], authorization.DefaultPolicy.AuthenticationSchemes);
        Assert.IsType<DenyAnonymousAuthorizationRequirement>(Assert.Single(authorization.DefaultPolicy.Requirements));
        Assert.Null(authorization.FallbackPolicy);
        Assert.Equal(typeof(AccessTokenValidationEvents), bearer.EventsType);
        Assert.Equal([SecurityAlgorithms.HmacSha256], bearer.TokenValidationParameters.ValidAlgorithms);
        Assert.Equal(TimeSpan.FromSeconds(30), bearer.TokenValidationParameters.ClockSkew);
    }

    [Theory]
    [InlineData("Audience", null)]
    [InlineData("Audience", " ")]
    [InlineData("Issuer", null)]
    [InlineData("Issuer", " ")]
    [InlineData("LifetimeInMinutes", "0")]
    [InlineData("LifetimeInMinutes", "61")]
    [InlineData("RefreshTokenLifetimeInDays", "0")]
    [InlineData("RefreshTokenLifetimeInDays", "31")]
    [InlineData("SecretKey", null)]
    [InlineData("SecretKey", "")]
    [InlineData("SecretKey", "                               ")]
    [InlineData("SecretKey", "1234567890123456789012345678901")]
    [InlineData("RefreshTokenSecretKey", null)]
    [InlineData("RefreshTokenSecretKey", "")]
    [InlineData("RefreshTokenSecretKey", "1234567890123456789012345678901")]
    public void StartupValidation_RejectsInvalidSettingsWithoutLeakingValues(string property, string? value)
    {
        using var provider = CreateProvider(property, value);

        var exception = Assert.Throws<OptionsValidationException>(
            () => provider.GetRequiredService<IStartupValidator>().Validate());

        Assert.Contains(property, exception.Message);
        Assert.DoesNotContain(new string('a', 64), exception.Message);
        Assert.DoesNotContain(new string('b', 64), exception.Message);
        if (property.EndsWith("SecretKey", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(value))
        {
            Assert.DoesNotContain(value, exception.Message);
        }
    }

    [Theory]
    [InlineData("12345678901234567890123456789012")]
    [InlineData("\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9\u00e9")]
    public void StartupValidation_AcceptsKeysWithAtLeast32Utf8Bytes(string key)
    {
        using var provider = CreateProvider("SecretKey", key);

        provider.GetRequiredService<IStartupValidator>().Validate();

        Assert.Equal(key, provider.GetRequiredService<IOptions<JwtOptions>>().Value.SecretKey);
    }

    [Fact]
    public void Registration_PreservesConfiguredAccountLockout()
    {
        using var provider = CreateProvider();
        var options = provider.GetRequiredService<IOptions<IdentityOptions>>().Value;

        Assert.True(options.Lockout.AllowedForNewUsers);
        Assert.Equal(7, options.Lockout.MaxFailedAccessAttempts);
        Assert.Equal(TimeSpan.FromMinutes(12), options.Lockout.DefaultLockoutTimeSpan);
    }

    private static ServiceProvider CreateProvider(string? property = null, string? value = null)
    {
        var settings = new Dictionary<string, string?>
        {
            ["JwtOptions:Issuer"] = "test-issuer",
            ["JwtOptions:Audience"] = "test-audience",
            ["JwtOptions:SecretKey"] = new string('a', 64),
            ["JwtOptions:RefreshTokenSecretKey"] = new string('b', 64),
            ["JwtOptions:LifetimeInMinutes"] = "15",
            ["JwtOptions:RefreshTokenLifetimeInDays"] = "7",
            ["AccountLockout:MaxFailedAccessAttempts"] = "7",
            ["AccountLockout:LockoutMinutes"] = "12",
        };
        if (property is not null)
        {
            settings[$"JwtOptions:{property}"] = value;
        }

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAdminAuthentication(new ConfigurationBuilder().AddInMemoryCollection(settings).Build());
        return services.BuildServiceProvider();
    }
}
