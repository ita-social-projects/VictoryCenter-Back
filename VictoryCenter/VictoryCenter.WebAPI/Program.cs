using dotenv.net;
using VictoryCenter.WebAPI.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                                                               ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set in configuration");
builder.Configuration["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_SECRETKEY")
                                                ?? throw new InvalidOperationException("JWTOPTIONS_SECRETKEY is not set in configuration");

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.ApplyMigrations();
await app.CreateInitialAdmin();

app.UseRequestResponseLogging();
app.UseCors();
app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program
{
}
