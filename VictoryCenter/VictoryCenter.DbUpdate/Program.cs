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

var connectionString = config.GetConnectionString("DefaultConnection");

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

using var context = new VictoryCenterDbContext(optionsBuilder.Options);

context.Database.Migrate();