using dotenv.net;
using VictoryCenter.WebAPI.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

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
