using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VictoryCenter.BLL;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using VictoryCenter.WebAPI.Factories;

namespace VictoryCenter.WebAPI.Extensions;

public static class ServicesConfiguration
{
    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<VictoryCenterDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(typeof(VictoryCenterDbContext).Assembly.GetName().Name);
                opt.MigrationsHistoryTable("__EFMigrationsHistory", "entity_framework");
            });
        });
    }

    public static void AddCustomServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddAutoMapper(
            cfg => { cfg.ConstructServicesUsing(type => services.BuildServiceProvider().GetRequiredService(type)); },
            typeof(BllAssemblyMarker).Assembly);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(BllAssemblyMarker).Assembly));

        services.AddValidatorsFromAssemblyContaining<BllAssemblyMarker>();

        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
        services.ConfigureBlob(configuration);
    }

    public static void MapOpenApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "VictoryCenter API V1");
            c.RoutePrefix = "swagger";
        });
    }

    public static async Task ApplyMigrations(this WebApplication app)
    {
        ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
        try
        {
            using IServiceScope localScope = app.Services.CreateScope();
            VictoryCenterDbContext victoryCenterDbContext =
                localScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            IEnumerable<string> pendingMigrations = await victoryCenterDbContext.Database.GetPendingMigrationsAsync();
            var migrations = pendingMigrations.ToList();

            if (migrations.Any())
            {
                logger.LogInformation("Pending migrations: {PendingMigrations}", string.Join(", ", migrations));
                IEnumerable<string> appliedMigrationsBefore =
                    await victoryCenterDbContext.Database.GetAppliedMigrationsAsync();

                await victoryCenterDbContext.Database.MigrateAsync();

                logger.LogInformation("Migrations applied successfully.");
                IEnumerable<string> appliedMigrationsAfter =
                    await victoryCenterDbContext.Database.GetAppliedMigrationsAsync();
                IEnumerable<string> newlyAppliedMigrations = appliedMigrationsAfter.Except(appliedMigrationsBefore);
                var appliedMigrations = newlyAppliedMigrations.ToList();

                if (appliedMigrations.Any())
                {
                    logger.LogInformation("Newly applied migrations: {NewlyAppliedMigrations}", string.Join(", ", appliedMigrations));
                }
                else
                {
                    logger.LogInformation("No new migrations were applied.");
                }
            }
            else
            {
                logger.LogInformation("No pending migrations.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during startup migration");
        }
    }

    private static void AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "VictoryCenter API",
                Version = "v1"
            });
        });
    }

    private static IServiceCollection ConfigureBlob(this IServiceCollection services, IConfiguration configuration)
    {
        var blobSection = configuration.GetSection("BlobEnvironmentVariables");
        var serviceType = blobSection.GetValue<string>("ServiceType");

        switch (serviceType)
        {
            case "Local":
                services.Configure<BlobEnvironmentVariables>(blobSection.GetSection("Local"));
                services.AddScoped<IBlobService, BlobService>();
                break;

            case "Azure":
                services.Configure<BlobEnvironmentVariables>(blobSection.GetSection("Azure"));
                services.AddScoped<IBlobService, BlobService>();
                break;

            default:
                throw new InvalidOperationException($"Unsupported Blob Service Type: {serviceType}");
        }

        return services;
    }
}
