using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Auth;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.RefreshTokenSessions;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Auth;

public class AuthControllerTests : BaseTestClass
{
    private const string TestEmail = "testadmin@victorycenter.com";
    private const string TestPassword = "TestPassword123!";
    private const string LoginPath = "/api/auth/login";
    private const string RefreshTokenPath = "/api/auth/refresh-token";
    private const string LogoutPath = "/api/auth/logout";
    private const string AdminEndpointPath = "/api/TeamCategories";

    public AuthControllerTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAuthResponse()
    {
        var request = new LoginRequestDto(TestEmail, TestPassword);
        var response = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, request);
        response.EnsureSuccessStatusCode();
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.False(string.IsNullOrEmpty(authResponse!.AccessToken));
        var setCookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault(h => h.StartsWith($"{AuthConstants.RefreshTokenCookieName}="));
        Assert.False(string.IsNullOrEmpty(setCookie));

        var issuedRefreshToken = setCookie!.Split(';')[0].Split('=', 2)[1];
        Fixture.DbContext.ChangeTracker.Clear();
        var admin = await Fixture.DbContext.Users.SingleAsync(user => user.Email == TestEmail);
        Assert.NotEqual(issuedRefreshToken, admin.RefreshToken);
        Assert.Matches("^[A-F0-9]{64}$", admin.RefreshToken!);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto(TestEmail, "WrongPassword!");
        var response = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsSameResponseAsInvalidPassword()
    {
        var invalidPasswordRequest = new LoginRequestDto(TestEmail, "WrongPassword!");
        var unknownEmailRequest = new LoginRequestDto("unknown@victorycenter.com", "WrongPassword!");

        using var invalidPasswordResponse = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, invalidPasswordRequest);
        using var unknownEmailResponse = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, unknownEmailRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, invalidPasswordResponse.StatusCode);
        Assert.Equal(invalidPasswordResponse.StatusCode, unknownEmailResponse.StatusCode);
        var invalidPasswordProblem = await invalidPasswordResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        var unknownEmailProblem = await unknownEmailResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal((int)HttpStatusCode.Unauthorized, invalidPasswordProblem!.Status);
        Assert.Equal("Unauthorized", invalidPasswordProblem.Title);
        Assert.Equal(AuthConstants.InvalidCredentials, invalidPasswordProblem.Detail);
        Assert.Equal(invalidPasswordProblem!.Status, unknownEmailProblem!.Status);
        Assert.Equal(invalidPasswordProblem.Title, unknownEmailProblem.Title);
        Assert.Equal(invalidPasswordProblem.Detail, unknownEmailProblem.Detail);
    }

    [Fact]
    public async Task RefreshToken_ValidCredentials_ReturnsAuthResponse()
    {
        var loginRequest = new LoginRequestDto(TestEmail, TestPassword);
        var loginResponse = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var setCookieHeaders = loginResponse.Headers.TryGetValues("Set-Cookie", out var values) ? values : null;
        var refreshTokenCookie = setCookieHeaders?.FirstOrDefault(h => h.StartsWith($"{AuthConstants.RefreshTokenCookieName}="));
        Assert.False(string.IsNullOrEmpty(refreshTokenCookie));

        var cookieHeader = refreshTokenCookie!.Split(';')[0];

        var request = new HttpRequestMessage(HttpMethod.Post, RefreshTokenPath);
        request.Headers.Add("Cookie", cookieHeader);

        var refreshResponse = await Fixture.HttpClient.SendAsync(request);
        refreshResponse.EnsureSuccessStatusCode();
        var refreshAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.False(string.IsNullOrEmpty(refreshAuthResponse!.AccessToken));
    }

    [Fact]
    public async Task RefreshToken_AfterLogin_AutomaticallySendsPathScopedCookie()
    {
        using var client = Fixture.Factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });

        using var loginResponse = await client.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto(TestEmail, TestPassword));
        loginResponse.EnsureSuccessStatusCode();

        using var refreshResponse = await client.PostAsync(RefreshTokenPath, content: null);

        refreshResponse.EnsureSuccessStatusCode();
        var authResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.False(string.IsNullOrEmpty(authResponse!.AccessToken));
    }

    [Fact]
    public async Task RefreshToken_InvalidRefreshToken_ReturnsUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, RefreshTokenPath);
        request.Headers.Add("Cookie", $"{AuthConstants.RefreshTokenCookieName}=invalidRefreshToken");

        var response = await Fixture.HttpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ReusedRotatedToken_ReturnsUnauthorized()
    {
        var loginResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto(TestEmail, TestPassword));
        loginResponse.EnsureSuccessStatusCode();
        var originalCookie = GetRefreshTokenCookie(loginResponse);

        using var firstRefreshRequest = CreateRefreshRequest(originalCookie);
        using var firstRefreshResponse = await Fixture.HttpClient.SendAsync(firstRefreshRequest);
        firstRefreshResponse.EnsureSuccessStatusCode();
        var rotatedCookie = GetRefreshTokenCookie(firstRefreshResponse);

        using (var scope = Fixture.Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            var originalToken = originalCookie.Split(';')[0].Split('=', 2)[1];
            var tokenHash = tokenService.HashRefreshToken(originalToken);
            var session = await dbContext.RefreshTokenSessions.SingleAsync(s => s.TokenHash == tokenHash);
            session.RevokedAt = DateTimeOffset.UtcNow - AuthConstants.RotationGracePeriod - TimeSpan.FromSeconds(5);
            await dbContext.SaveChangesAsync();
        }

        using var replayRequest = CreateRefreshRequest(originalCookie);
        using var replayResponse = await Fixture.HttpClient.SendAsync(replayRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, replayResponse.StatusCode);

        using var rotatedTokenRequest = CreateRefreshRequest(rotatedCookie);
        using var rotatedTokenResponse = await Fixture.HttpClient.SendAsync(rotatedTokenRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, rotatedTokenResponse.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ReplayedWithinGracePeriod_DoesNotRevokeFamily()
    {
        var loginResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto(TestEmail, TestPassword));
        loginResponse.EnsureSuccessStatusCode();
        var originalCookie = GetRefreshTokenCookie(loginResponse);

        using var firstRefreshRequest = CreateRefreshRequest(originalCookie);
        using var firstRefreshResponse = await Fixture.HttpClient.SendAsync(firstRefreshRequest);
        firstRefreshResponse.EnsureSuccessStatusCode();
        var rotatedCookie = GetRefreshTokenCookie(firstRefreshResponse);

        // Immediate replay within grace period returns Unauthorized but preserves the family
        using var replayRequest = CreateRefreshRequest(originalCookie);
        using var replayResponse = await Fixture.HttpClient.SendAsync(replayRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, replayResponse.StatusCode);

        // Rotated cookie remains valid because the family was not revoked
        using var rotatedTokenRequest = CreateRefreshRequest(rotatedCookie);
        using var rotatedTokenResponse = await Fixture.HttpClient.SendAsync(rotatedTokenRequest);
        rotatedTokenResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Login_FromSecondDevice_InvalidatesFirstDeviceRefreshToken()
    {
        var firstLoginResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto(TestEmail, TestPassword));
        firstLoginResponse.EnsureSuccessStatusCode();
        var firstDeviceCookie = GetRefreshTokenCookie(firstLoginResponse);

        var secondLoginResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto(TestEmail, TestPassword));
        secondLoginResponse.EnsureSuccessStatusCode();
        var secondDeviceCookie = GetRefreshTokenCookie(secondLoginResponse);

        using var firstDeviceRefreshRequest = CreateRefreshRequest(firstDeviceCookie);
        using var firstDeviceRefreshResponse = await Fixture.HttpClient.SendAsync(firstDeviceRefreshRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, firstDeviceRefreshResponse.StatusCode);

        using var secondDeviceRefreshRequest = CreateRefreshRequest(secondDeviceCookie);
        using var secondDeviceRefreshResponse = await Fixture.HttpClient.SendAsync(secondDeviceRefreshRequest);
        secondDeviceRefreshResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Logout_AfterSuccessfulLogin_ReturnsOkAndClearsRefreshTokenCookie()
    {
        var loginRequest = new LoginRequestDto(TestEmail, TestPassword);
        var loginResponse = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        var accessToken = authResponse!.AccessToken;

        var setCookieHeaders = loginResponse.Headers.TryGetValues("Set-Cookie", out var values) ? values : null;
        var refreshTokenCookie = setCookieHeaders?.FirstOrDefault(h => h.StartsWith($"{AuthConstants.RefreshTokenCookieName}="));
        Assert.False(string.IsNullOrEmpty(refreshTokenCookie));

        var cookieHeader = refreshTokenCookie!.Split(';')[0];

        var logoutRequest = new HttpRequestMessage(HttpMethod.Post, LogoutPath);
        logoutRequest.Headers.Add("Cookie", cookieHeader);
        logoutRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var logoutResponse = await Fixture.HttpClient.SendAsync(logoutRequest);

        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        var logoutSetCookieHeaders = logoutResponse.Headers.TryGetValues("Set-Cookie", out var logoutValues) ? logoutValues : null;
        var logoutRefreshTokenCookie = logoutSetCookieHeaders?.FirstOrDefault(h => h.StartsWith($"{AuthConstants.RefreshTokenCookieName}="));
        Assert.NotNull(logoutRefreshTokenCookie);
        Assert.Contains("expires=Thu, 01 Jan 1970", logoutRefreshTokenCookie);
    }

    [Fact]
    public async Task Logout_WithoutAuthorization_ReturnsUnauthorized()
    {
        using var anonymousClient = Fixture.Factory.CreateClient();
        var logoutRequest = new HttpRequestMessage(HttpMethod.Post, LogoutPath);
        var logoutResponse = await anonymousClient.SendAsync(logoutRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, logoutResponse.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_AfterLogout_ReturnsUnauthorized()
    {
        var loginResponse = await Fixture.HttpClient.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto(TestEmail, TestPassword));
        loginResponse.EnsureSuccessStatusCode();

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        var accessToken = authResponse!.AccessToken;
        var refreshTokenCookie = GetRefreshTokenCookie(loginResponse);

        var logoutRequest = new HttpRequestMessage(HttpMethod.Post, LogoutPath);
        logoutRequest.Headers.Add("Cookie", refreshTokenCookie);
        logoutRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var logoutResponse = await Fixture.HttpClient.SendAsync(logoutRequest);
        logoutResponse.EnsureSuccessStatusCode();

        using var refreshRequest = CreateRefreshRequest(refreshTokenCookie);
        using var refreshResponse = await Fixture.HttpClient.SendAsync(refreshRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesAccessTokenIssuedBeforeLogout()
    {
        var loginResponse = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, new LoginRequestDto(TestEmail, TestPassword));
        loginResponse.EnsureSuccessStatusCode();
        var accessToken = (await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>())!.AccessToken;

        using var requestBeforeLogout = CreateAuthorizedRequest(HttpMethod.Get, AdminEndpointPath, accessToken);
        using var responseBeforeLogout = await Fixture.HttpClient.SendAsync(requestBeforeLogout);
        Assert.Equal(HttpStatusCode.OK, responseBeforeLogout.StatusCode);

        using var logoutRequest = CreateAuthorizedRequest(HttpMethod.Post, LogoutPath, accessToken);
        using var logoutResponse = await Fixture.HttpClient.SendAsync(logoutRequest);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        using var requestAfterLogout = CreateAuthorizedRequest(HttpMethod.Get, AdminEndpointPath, accessToken);
        using var responseAfterLogout = await Fixture.HttpClient.SendAsync(requestAfterLogout);
        Assert.Equal(HttpStatusCode.Unauthorized, responseAfterLogout.StatusCode);
    }

    [Fact]
    public async Task Logout_WithStaleConcurrencyStamp_RetriesAndRotatesSecurityStamp()
    {
        var customFactory = Fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(d => d.ServiceType == typeof(IRepositoryWrapper));
                services.Remove(descriptor);
                services.AddScoped<IRepositoryWrapper, ConcurrencySimulatingRepositoryWrapper>();
            });
        });
        using var client = customFactory.CreateClient();

        using (var scope = customFactory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<AdminUser>>();
            var existing = await userManager.FindByEmailAsync(TestEmail);
            if (existing == null)
            {
                var admin = new AdminUser
                {
                    UserName = TestEmail,
                    Email = TestEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    RefreshToken = "refresh_token",
                    RefreshTokenValidTo = DateTimeOffset.MaxValue
                };
                var createResult = await userManager.CreateAsync(admin, TestPassword);
                Assert.True(createResult.Succeeded);
            }
        }

        var loginResponse = await client.PostAsJsonAsync(
            LoginPath,
            new LoginRequestDto(TestEmail, TestPassword));
        loginResponse.EnsureSuccessStatusCode();

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        var accessToken = authResponse!.AccessToken;
        var refreshTokenCookie = GetRefreshTokenCookie(loginResponse);

        string? initialSecurityStamp;
        using (var scope = customFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var initialAdmin = await dbContext.Users.AsNoTracking().SingleAsync(u => u.Email == TestEmail);
            initialSecurityStamp = initialAdmin.SecurityStamp;
        }

        Assert.NotNull(initialSecurityStamp);

        ConcurrencySimulatingRepositoryWrapper.ShouldSimulate = true;
        try
        {
            using var logoutRequest = CreateAuthorizedRequest(HttpMethod.Post, LogoutPath, accessToken);
            logoutRequest.Headers.Add("Cookie", refreshTokenCookie);
            using var logoutResponse = await client.SendAsync(logoutRequest);

            Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
        }
        finally
        {
            ConcurrencySimulatingRepositoryWrapper.ShouldSimulate = false;
        }

        using (var scope = customFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var updatedAdmin = await dbContext.Users.AsNoTracking().SingleAsync(u => u.Email == TestEmail);
            Assert.NotEqual(initialSecurityStamp, updatedAdmin.SecurityStamp);
            Assert.Null(updatedAdmin.RefreshToken);
        }

        using var subsequentRequest = CreateAuthorizedRequest(HttpMethod.Get, AdminEndpointPath, accessToken);
        using var subsequentResponse = await client.SendAsync(subsequentRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, subsequentResponse.StatusCode);
    }

    [Fact]
    public async Task AdminEndpoint_WithAccessTokenWithoutSecurityStampClaim_ReturnsUnauthorized()
    {
        var tokenService = Fixture.Factory.Services.GetRequiredService<ITokenService>();
        var accessToken = tokenService.CreateAccessToken([new Claim(ClaimTypes.Email, TestEmail)]);

        using var request = CreateAuthorizedRequest(HttpMethod.Get, AdminEndpointPath, accessToken);
        using var response = await Fixture.HttpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string path, string accessToken)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return request;
    }

    private static string GetRefreshTokenCookie(HttpResponseMessage response)
    {
        var setCookieHeaders = response.Headers.GetValues("Set-Cookie");
        return setCookieHeaders.Single(header => header.StartsWith($"{AuthConstants.RefreshTokenCookieName}="))
            .Split(';')[0];
    }

    private static HttpRequestMessage CreateRefreshRequest(string refreshTokenCookie)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, RefreshTokenPath);
        request.Headers.Add("Cookie", refreshTokenCookie);
        return request;
    }

    private sealed class ConcurrencySimulatingRepositoryWrapper : RepositoryWrapper, IRepositoryWrapper
    {
        private static bool _shouldSimulate;
        private readonly IServiceProvider _serviceProvider;
        private int _simulationExecuted;
        private IRefreshTokenSessionRepository? _simulatedSessionRepository;

        public ConcurrencySimulatingRepositoryWrapper(
            VictoryCenterDbContext context,
            IServiceProvider serviceProvider)
            : base(context)
        {
            _serviceProvider = serviceProvider;
        }

        public static bool ShouldSimulate
        {
            get => _shouldSimulate;
            set => _shouldSimulate = value;
        }

        IRefreshTokenSessionRepository IRepositoryWrapper.RefreshTokenSessionRepository
        {
            get
            {
                if (_simulatedSessionRepository != null)
                {
                    return _simulatedSessionRepository;
                }

                var target = RefreshTokenSessionRepository;
                var proxy = DispatchProxy.Create<IRefreshTokenSessionRepository, SessionRepoDispatchProxy>();
                ((SessionRepoDispatchProxy)(object)proxy).Initialize(target, () =>
                {
                    if (ShouldSimulate && Interlocked.Exchange(ref _simulationExecuted, 1) == 0)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
                        var adminInDb = dbContext.Users.First(u => u.Email == TestEmail);
                        adminInDb.ConcurrencyStamp = Guid.NewGuid().ToString();
                        dbContext.SaveChanges();
                    }
                });

                _simulatedSessionRepository = proxy;
                return _simulatedSessionRepository;
            }
        }

        public class SessionRepoDispatchProxy : DispatchProxy
        {
            private IRefreshTokenSessionRepository _target = null!;
            private Action _onRevoke = null!;

            public void Initialize(IRefreshTokenSessionRepository target, Action onRevoke)
            {
                _target = target;
                _onRevoke = onRevoke;
            }

#pragma warning disable SA1011 // Closing square bracket should be followed by a space
            protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
#pragma warning restore SA1011
            {
                var result = targetMethod!.Invoke(_target, args);
                if (targetMethod.Name == nameof(IRefreshTokenSessionRepository.RevokeAdminSessionsAsync))
                {
                    if (result is Task task)
                    {
                        return InterceptAsync(task);
                    }
                }

                return result;
            }

            private async Task InterceptAsync(Task task)
            {
                await task;
                _onRevoke();
            }
        }
    }
}
