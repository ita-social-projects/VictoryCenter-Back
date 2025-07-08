using System.Net;
using System.Net.Http.Json;
using VictoryCenter.BLL.DTOs.Auth;
using VictoryCenter.IntegrationTests.ControllerTests.Base;

namespace VictoryCenter.IntegrationTests.ControllerTests.Auth;

[Collection("SharedIntegrationTests")]
public class AuthControllerTests : IClassFixture<IntegrationTestDbFixture>
{
    private const string TestEmail = "testadmin@victorycenter.com";
    private const string TestPassword = "TestPassword123!";
    private readonly IntegrationTestDbFixture _fixture;

    public AuthControllerTests(IntegrationTestDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Login_Success_ReturnsAuthResponse()
    {
        var request = new LoginRequestDto(TestEmail, TestPassword);
        var response = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/login", request);
        response.EnsureSuccessStatusCode();
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.False(string.IsNullOrEmpty(authResponse.AccessToken));
        var setCookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault(h => h.StartsWith("refreshToken="));
        Assert.False(string.IsNullOrEmpty(setCookie));
    }

    [Fact]
    public async Task Login_Failure_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto(TestEmail, "WrongPassword!");
        var response = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/login", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_Success_ReturnsAuthResponse()
    {
        var loginRequest = new LoginRequestDto(TestEmail, TestPassword);
        var loginResponse = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var setCookieHeaders = loginResponse.Headers.TryGetValues("Set-Cookie", out var values) ? values : null;
        var refreshTokenCookie = setCookieHeaders?.FirstOrDefault(h => h.StartsWith("refreshToken="));
        Assert.False(string.IsNullOrEmpty(refreshTokenCookie));

        var cookieHeader = refreshTokenCookie!.Split(';')[0];

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh-token");
        request.Headers.Add("Cookie", cookieHeader);

        var refreshResponse = await _fixture.HttpClient.SendAsync(request);
        refreshResponse.EnsureSuccessStatusCode();
        var refreshAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.False(string.IsNullOrEmpty(refreshAuthResponse.AccessToken));
    }

    [Fact]
    public async Task RefreshToken_Failure_ReturnsUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh-token");
        request.Headers.Add("Cookie", "refreshToken=invalidRefreshToken");

        var response = await _fixture.HttpClient.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
