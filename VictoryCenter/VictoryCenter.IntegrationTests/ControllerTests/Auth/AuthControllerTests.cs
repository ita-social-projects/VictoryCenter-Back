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
        var request = new LoginRequest(TestEmail, TestPassword);
        var response = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/login", request);
        response.EnsureSuccessStatusCode();
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrEmpty(authResponse.AccessToken));
        Assert.False(string.IsNullOrEmpty(authResponse.RefreshToken));
    }

    [Fact]
    public async Task Login_Failure_ReturnsUnauthorized()
    {
        var request = new LoginRequest(TestEmail, "WrongPassword!");
        var response = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/login", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_Success_ReturnsAuthResponse()
    {
        var loginRequest = new LoginRequest(TestEmail, TestPassword);
        var loginResponse = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var refreshRequest = new RefreshTokenRequest(authResponse.AccessToken, authResponse.RefreshToken);
        var refreshResponse = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);
        refreshResponse.EnsureSuccessStatusCode();
        var refreshAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrEmpty(refreshAuthResponse.AccessToken));
        Assert.False(string.IsNullOrEmpty(refreshAuthResponse.RefreshToken));
    }

    [Fact]
    public async Task RefreshToken_Failure_ReturnsUnauthorized()
    {
        var refreshRequest = new RefreshTokenRequest("invalidAccessToken", "invalidRefreshToken");
        var response = await _fixture.HttpClient.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
