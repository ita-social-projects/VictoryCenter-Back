using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("VICTORYCENTER_")
    .Build();

var connectionString = config.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

Console.WriteLine($"Using connection string: {connectionString}");

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});

var optionsBuilder = new DbContextOptionsBuilder<VictoryCenterDbContext>();
optionsBuilder
    .UseSqlServer(connectionString)
    .UseLoggerFactory(loggerFactory);

try
{
    using var context = new VictoryCenterDbContext(optionsBuilder.Options);
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    await Console.Error.WriteLineAsync($"Migration failed: {ex}");
    Environment.ExitCode = 1;
}
