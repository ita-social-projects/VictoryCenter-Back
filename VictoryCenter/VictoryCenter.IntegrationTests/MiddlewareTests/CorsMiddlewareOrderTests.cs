using System.Net;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.MiddlewareTests;

[Collection("SharedIntegrationTests")]
public class CorsMiddlewareOrderTests
{
    private const string AllowedOrigin = "https://localhost:3000";
    private const string DisallowedOrigin = "https://evil.example.com";
    private const string AllowOriginHeader = "Access-Control-Allow-Origin";
    private const string AllowCredentialsHeader = "Access-Control-Allow-Credentials";

    private readonly HttpClient _client;

    public CorsMiddlewareOrderTests(IntegrationTestDbFixture fixture)
    {
        var customFactory = fixture.Factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("CORS:AllowedOrigins:0", AllowedOrigin);

            builder.ConfigureServices(services =>
            {
                services
                    .AddControllers()
                    .AddApplicationPart(typeof(FakeErrorController).Assembly)
                    .AddControllersAsServices();
            });
        });

        _client = customFactory.CreateClient();
    }

    [Fact]
    public async Task ServerError_WithAllowedOrigin_ShouldIncludeCorsHeaders()
    {
        var response = await SendAsync("/api/Test/ThrowException", AllowedOrigin);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(AllowedOrigin, response.Headers.GetValues(AllowOriginHeader).Single());
        Assert.Equal("true", response.Headers.GetValues(AllowCredentialsHeader).Single());
    }

    [Fact]
    public async Task ValidationError_WithAllowedOrigin_ShouldIncludeCorsHeaders()
    {
        var response = await SendAsync("/api/Test/ThrowValidationException", AllowedOrigin);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(AllowedOrigin, response.Headers.GetValues(AllowOriginHeader).Single());
    }

    [Fact]
    public async Task ServerError_WithDisallowedOrigin_ShouldNotIncludeCorsHeaders()
    {
        var response = await SendAsync("/api/Test/ThrowException", DisallowedOrigin);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(response.Headers.Contains(AllowOriginHeader));
    }

    [Fact]
    public async Task SuccessfulResponse_WithAllowedOrigin_ShouldKeepCorsHeaders()
    {
        var response = await SendAsync("/health/live", AllowedOrigin);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(AllowedOrigin, response.Headers.GetValues(AllowOriginHeader).Single());
    }

    private Task<HttpResponseMessage> SendAsync(string path, string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Add("Origin", origin);
        return _client.SendAsync(request);
    }
}
