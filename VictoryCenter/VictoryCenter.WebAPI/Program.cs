using dotenv.net;
using Microsoft.AspNetCore.HttpOverrides;
using VictoryCenter.WebAPI.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureApplication(builder);
builder.Configuration.AddLocalEnvironmentVariables();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddOpenTelemetryTracing();
builder.Logging.AddOpenTelemetryLogging();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRateLimiterConfiguration();

var app = builder.Build();

app.MapOpenApi();

await app.ApplyMigrationsAsync();
await app.CreateInitialDataAsync();

app.UseRequestResponseLogging();
app.UseCors();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseRateLimiter();
app.MapControllers();
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.Run();

public partial class Program
{
}
