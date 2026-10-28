using dotenv.net;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using VictoryCenter.WebAPI.Extensions;
using VictoryCenter.BLL.Hubs;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
using var configurationFileProvider = builder.Host.ConfigureApplication(builder);
builder.Configuration.AddLocalEnvironmentVariables();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddOpenTelemetryTracing();
builder.Logging.AddOpenTelemetryLogging();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRateLimiterConfiguration();
builder.Services.AddForwardedHeadersConfiguration(builder.Configuration);

var app = builder.Build();
app.Lifetime.ApplicationStopped.Register(configurationFileProvider.Dispose);

app.MapOpenApi();

await app.ApplyMigrationsAsync();
await app.CreateInitialDataAsync();

app.UseForwardedHeaders();
app.UseCors();
app.UseExceptionHandling();
app.UseRequestResponseLogging();
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
});
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapHub<PdfReportsHub>("/hubs/reports");

app.Run();

public partial class Program
{
}
