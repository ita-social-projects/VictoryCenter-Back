using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VictoryCenter.BLL;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
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
                opt.MigrationsHistoryTable("__EFMigrationsHistory", schema: "entity_framework");
            });
        });

        services.AddIdentity<Admin, IdentityRole<int>>()
            .AddEntityFrameworkStores<VictoryCenterDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = Constants.Authentication.GetDefaultTokenValidationParameters(configuration);
            });
    }

    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddAutoMapper(typeof(BllAssemblyMarker).Assembly);
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
        services.AddScoped<IPagesService, PagesService>();
        services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.Position)
            .ValidateDataAnnotations()
            .ValidateOnStart();
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
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        try
        {
            using IServiceScope localScope = app.Services.CreateScope();
            var victoryCenterDbContext = localScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var pendingMigrations = await victoryCenterDbContext.Database.GetPendingMigrationsAsync();
            var migrations = pendingMigrations.ToList();

            if (migrations.Any())
            {
                logger.LogInformation("Pending migrations: {PendingMigrations}", string.Join(", ", migrations));
                var appliedMigrationsBefore = await victoryCenterDbContext.Database.GetAppliedMigrationsAsync();

                await victoryCenterDbContext.Database.MigrateAsync();

                logger.LogInformation("Migrations applied successfully.");
                var appliedMigrationsAfter = await victoryCenterDbContext.Database.GetAppliedMigrationsAsync();
                var newlyAppliedMigrations = appliedMigrationsAfter.Except(appliedMigrationsBefore);
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
}
