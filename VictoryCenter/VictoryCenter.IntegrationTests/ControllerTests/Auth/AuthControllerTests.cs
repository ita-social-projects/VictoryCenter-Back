using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        using var replayRequest = CreateRefreshRequest(originalCookie);
        using var replayResponse = await Fixture.HttpClient.SendAsync(replayRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, replayResponse.StatusCode);

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
        var logoutRequest = new HttpRequestMessage(HttpMethod.Post, LogoutPath);
        var logoutResponse = await Fixture.HttpClient.SendAsync(logoutRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, logoutResponse.StatusCode);
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
}
