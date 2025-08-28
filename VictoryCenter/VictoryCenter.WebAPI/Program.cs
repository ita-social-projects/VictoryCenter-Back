using dotenv.net;
using VictoryCenter.WebAPI.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureApplication(builder);
builder.Configuration.AddLocalEnvironmentVariables();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddOpenTelemetryTracing();
builder.Logging.AddOpenTelemetryLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.ApplyMigrations();
await app.CreateInitialData();

app.UseRequestResponseLogging();
app.UseCors();
app.MapControllers();
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program
{
}
