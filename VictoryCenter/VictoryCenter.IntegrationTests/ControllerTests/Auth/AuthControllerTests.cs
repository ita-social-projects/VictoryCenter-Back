using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Auth;
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
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto(TestEmail, "WrongPassword!");
        var response = await Fixture.HttpClient.PostAsJsonAsync(LoginPath, request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
    public async Task RefreshToken_InvalidRefreshToken_ReturnsUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, RefreshTokenPath);
        request.Headers.Add("Cookie", $"{AuthConstants.RefreshTokenCookieName}=invalidRefreshToken");

        var response = await Fixture.HttpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
        var logoutRequest = new HttpRequestMessage(HttpMethod.Post, LogoutPath);
        var logoutResponse = await Fixture.HttpClient.SendAsync(logoutRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, logoutResponse.StatusCode);
    }
}
