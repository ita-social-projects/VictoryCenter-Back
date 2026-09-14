using dotenv.net;
using VictoryCenter.WebAPI.Extensions;
using VictoryCenter.BLL.Hubs;

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
builder.Services.AddForwardedHeadersConfiguration(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();

await app.ApplyMigrationsAsync();
await app.CreateInitialDataAsync();

app.UseForwardedHeaders();
app.UseRequestResponseLogging();
app.UseCors();
app.UseRateLimiter();
app.MapControllers();
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
