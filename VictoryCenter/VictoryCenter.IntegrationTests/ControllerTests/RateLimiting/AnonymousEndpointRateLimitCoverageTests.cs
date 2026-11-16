using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.RateLimiting;

/// <summary>
/// Fails when an endpoint that anonymous clients can reach has no rate-limiting policy,
/// so new public endpoints can't ship without one.
/// </summary>
[Collection("SharedIntegrationTests")]
public class AnonymousEndpointRateLimitCoverageTests(IntegrationTestDbFixture fixture)
{
    // Anonymous endpoints that are intentionally not rate limited, with the reason.
    private static readonly Dictionary<string, string> ExcludedRoutes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/health/live"] = "Orchestrator liveness probe; must never be throttled.",
        ["/health/ready"] = "Orchestrator readiness probe; must never be throttled.",
    };

    [Fact]
    public void AllAnonymousEndpoints_ShouldHaveRateLimitingPolicy()
    {
        var endpoints = GetRouteEndpoints();

        var uncovered = endpoints
            .Where(IsAnonymous)
            .Where(endpoint => !ExcludedRoutes.ContainsKey(GetRoute(endpoint)))
            .Where(endpoint => !HasRateLimitingPolicy(endpoint))
            .Select(Describe)
            .OrderBy(description => description)
            .ToList();

        Assert.True(
            uncovered.Count == 0,
            "Anonymous endpoints without a rate-limiting policy. Add [EnableRateLimiting] (or inherit PublicApiController), " +
            $"or add the route to {nameof(ExcludedRoutes)} with a reason:{Environment.NewLine}" +
            string.Join(Environment.NewLine, uncovered));
    }

    [Fact]
    public void ExcludedRoutes_ShouldExistAndBeAnonymous()
    {
        var anonymousRoutes = GetRouteEndpoints()
            .Where(IsAnonymous)
            .Select(GetRoute)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.All(ExcludedRoutes.Keys, route => Assert.Contains(route, anonymousRoutes));
    }

    [Fact]
    public void Guard_ShouldSeeKnownAnonymousEndpoints()
    {
        var anonymousRoutes = GetRouteEndpoints()
            .Where(IsAnonymous)
            .Select(GetRoute)
            .ToList();

        Assert.Contains("/api/auth/login", anonymousRoutes, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("/api/MainPage", anonymousRoutes, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(anonymousRoutes, route => route.StartsWith("/hubs/reports", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsAnonymous(Endpoint endpoint)
    {
        var metadata = endpoint.Metadata;
        return metadata.GetMetadata<IAllowAnonymous>() is not null
            || metadata.GetMetadata<IAuthorizeData>() is null;
    }

    private static bool HasRateLimitingPolicy(Endpoint endpoint)
    {
        var metadata = endpoint.Metadata;
        return metadata.GetMetadata<EnableRateLimitingAttribute>() is not null
            && metadata.GetMetadata<DisableRateLimitingAttribute>() is null;
    }

    private static string GetRoute(RouteEndpoint endpoint) => "/" + endpoint.RoutePattern.RawText?.TrimStart('/');

    private static string Describe(RouteEndpoint endpoint)
    {
        var methods = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods ?? ["*"];
        return $"{string.Join(",", methods)} {GetRoute(endpoint)} ({endpoint.DisplayName})";
    }

    private List<RouteEndpoint> GetRouteEndpoints()
    {
        return
        [
            .. fixture.Factory.Services
                .GetRequiredService<EndpointDataSource>()
                .Endpoints
                .OfType<RouteEndpoint>(),
        ];
    }
}
