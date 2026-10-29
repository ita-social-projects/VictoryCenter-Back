using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

[Collection("SharedIntegrationTests")]
public class SwaggerAuthMiddlewareTest(IntegrationTestDbFixture fixture)
{
    private const string SwaggerUrl = "/swagger/index.html";
    private const string SwaggerJsonUrl = "/swagger/v1/swagger.json";
    private const int SwaggerRequestLimit = 60;

    [Fact]
    public async Task Development_WithoutCredentials_ShouldReturn200()
    {
        // Arrange
        using var client = CreateClient("Development");

        // Act
        var response = await client.GetAsync(SwaggerUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Development_Over60Requests_ShouldNotBeRateLimited()
    {
        // Arrange
        using var client = CreateClient("Development");

        // Act, Assert
        for (var i = 0; i < 75; i++)
        {
            var response = await client.GetAsync(SwaggerUrl);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Theory]
    [InlineData(SwaggerUrl)]
    [InlineData(SwaggerJsonUrl)]
    public async Task Production_WithoutCredentials_ShouldReturn401(string url)
    {
        // Arrange
        using var client = CreateClient("Production");

        // Act
        var response = await client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains(
            response.Headers.WwwAuthenticate,
            header => header.Scheme == "Basic");
    }

    [Fact]
    public async Task Production_InvalidCredentials_ShouldReturn401()
    {
        // Arrange
        using var client = CreateClient("Production");

        SetBasicAuth(client, "invalid@example.com", "wrong-password");

        // Act
        var response = await client.GetAsync(SwaggerUrl);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Production_Over60Requests_ShouldReturn429()
    {
        // Arrange
        using var client = CreateClient("Production");

        // Act, Assert
        for (var i = 0; i < SwaggerRequestLimit; i++)
        {
            var response = await client.GetAsync(SwaggerUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        var responseAfterLimit = await client.GetAsync(SwaggerUrl);

        Assert.Equal(HttpStatusCode.TooManyRequests, responseAfterLimit.StatusCode);
    }

    [Fact]
    public async Task Production_OtherEndpoints_ShouldNotRequireSwaggerAuth()
    {
        // Arrange
        using var client = CreateClient("Production");

        // Act
        var response = await client.GetAsync("/non-existing-endpoint");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private HttpClient CreateClient(string environment)
    {
        var factory = fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                WebHostDefaults.EnvironmentKey,
                environment);
        });

        var client = factory.CreateClient();
        client.BaseAddress = new Uri("https://localhost");

        return client;
    }

    private static void SetBasicAuth(
        HttpClient client,
        string email,
        string password)
    {
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{email}:{password}"));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
    }
}
