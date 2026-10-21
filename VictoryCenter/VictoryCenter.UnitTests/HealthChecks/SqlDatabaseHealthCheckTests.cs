using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.WebAPI.HealthChecks;

namespace VictoryCenter.UnitTests.HealthChecks;

public class SqlDatabaseHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenScopeCreationFails_ReturnsUnhealthyWithoutExceptionDetails()
    {
        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(factory => factory.CreateScope())
            .Throws(new InvalidOperationException("Sensitive connection details"));
        var healthCheck = new SqlDatabaseHealthCheck(scopeFactory.Object, Mock.Of<ILogger<SqlDatabaseHealthCheck>>());

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("SQL database connection is unavailable", result.Description);
        Assert.Null(result.Exception);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckHealthAsync_WhenOperationIsCanceled_RespectsCallerCancellation(bool callerCanceled)
    {
        using var cancellation = new CancellationTokenSource();
        if (callerCanceled)
        {
            cancellation.Cancel();
        }

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(factory => factory.CreateScope())
            .Throws(new OperationCanceledException(cancellation.Token));
        var healthCheck = new SqlDatabaseHealthCheck(scopeFactory.Object, Mock.Of<ILogger<SqlDatabaseHealthCheck>>());

        if (callerCanceled)
        {
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                healthCheck.CheckHealthAsync(new HealthCheckContext(), cancellation.Token));
        }
        else
        {
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), cancellation.Token);

            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Null(result.Exception);
        }
    }
}
