using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Auth;

public class AuthenticationBoundaryTests : BaseTestClass
{
    public AuthenticationBoundaryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task AdminEndpoint_WithValidIdentityCookieButNoBearerToken_ReturnsUnauthorized()
    {
        using var client = Fixture.Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = false,
        });
        var options = Fixture.Factory.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(IdentityConstants.ApplicationScheme);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "1"), new Claim(ClaimTypes.Email, "testadmin@victorycenter.com")],
            IdentityConstants.ApplicationScheme));
        var ticket = new AuthenticationTicket(
            principal,
            new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            },
            IdentityConstants.ApplicationScheme);
        var cookie = options.TicketDataFormat.Protect(ticket);
        Assert.NotNull(options.TicketDataFormat.Unprotect(cookie));
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/TeamCategories");
        request.Headers.Add("Cookie", $"{options.Cookie.Name}={cookie}");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains(response.Headers.WwwAuthenticate, header => header.Scheme == "Bearer");
        Assert.Null(response.Headers.Location);
    }

    [Fact]
    public async Task PublicApiAndHealthProbes_WithoutAuthentication_RemainAccessible()
    {
        using var client = Fixture.Factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        foreach (var path in new[] { "/health/live", "/health/ready", "/api/EventNews/published", "/api/CompanyProfile" })
        {
            using var response = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
