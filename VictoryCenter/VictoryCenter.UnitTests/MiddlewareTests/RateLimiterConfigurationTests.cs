using System.Net;
using Microsoft.AspNetCore.Http;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.UnitTests.MiddlewareTests;

public class RateLimiterConfigurationTests
{
    [Theory]
    [InlineData("198.51.100.10", "198.51.100.10")]
    [InlineData("::ffff:198.51.100.10", "198.51.100.10")]
    [InlineData("2001:db8:1:2:aaaa:bbbb:cccc:dddd", "2001:db8:1:2::/64")]
    [InlineData("2001:db8:1:2::1", "2001:db8:1:2::/64")]
    public void GetClientPartitionKey_ReturnsNormalizedAddress(string remoteAddress, string expectedKey)
    {
        var httpContext = CreateContext(IPAddress.Parse(remoteAddress));

        var partitionKey = RateLimiterConfiguration.GetClientPartitionKey(httpContext);

        Assert.Equal(expectedKey, partitionKey);
    }

    [Fact]
    public void GetClientPartitionKey_IPv6AddressesInSamePrefix_ShareKey()
    {
        var first = CreateContext(IPAddress.Parse("2001:db8:1:2::1"));
        var second = CreateContext(IPAddress.Parse("2001:db8:1:2:ffff:ffff:ffff:ffff"));

        Assert.Equal(
            RateLimiterConfiguration.GetClientPartitionKey(first),
            RateLimiterConfiguration.GetClientPartitionKey(second));
    }

    [Fact]
    public void GetClientPartitionKey_IPv6AddressesInDifferentPrefixes_UseDifferentKeys()
    {
        var first = CreateContext(IPAddress.Parse("2001:db8:1:2::1"));
        var second = CreateContext(IPAddress.Parse("2001:db8:1:3::1"));

        Assert.NotEqual(
            RateLimiterConfiguration.GetClientPartitionKey(first),
            RateLimiterConfiguration.GetClientPartitionKey(second));
    }

    [Fact]
    public void GetClientPartitionKey_NoRemoteAddress_ReturnsUnknown()
    {
        var httpContext = CreateContext(null);

        var partitionKey = RateLimiterConfiguration.GetClientPartitionKey(httpContext);

        Assert.Equal("unknown", partitionKey);
    }

    [Fact]
    public void GetClientPartitionKey_IgnoresForwardingHeaders()
    {
        var httpContext = CreateContext(IPAddress.Parse("198.51.100.10"));
        httpContext.Request.Headers["X-Forwarded-For"] = "203.0.113.99";
        httpContext.Request.Headers["X-Real-IP"] = "203.0.113.98";

        var partitionKey = RateLimiterConfiguration.GetClientPartitionKey(httpContext);

        Assert.Equal("198.51.100.10", partitionKey);
    }

    [Fact]
    public void RateLimitingOptionsValidator_DefaultOptions_Succeeds()
    {
        var result = new RateLimitingOptionsValidator().Validate(null, new RateLimitingOptions());

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void RateLimitingOptionsValidator_NonPositiveValues_Fails()
    {
        var options = new RateLimitingOptions
        {
            AdminLogin = new FixedWindowPolicyOptions { PermitLimit = 0, WindowSeconds = 60 },
            ImageUpload = new ConcurrencyPolicyOptions { PermitLimit = 1, QueueLimit = -1 },
        };

        var result = new RateLimitingOptionsValidator().Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures!, failure => failure.Contains(nameof(RateLimitingOptions.AdminLogin)));
        Assert.Contains(result.Failures!, failure => failure.Contains(nameof(RateLimitingOptions.ImageUpload)));
    }

    private static DefaultHttpContext CreateContext(IPAddress? remoteAddress)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = remoteAddress;
        return httpContext;
    }
}
