using System.Reflection;
using MediatR;
using VictoryCenter.BLL;
using VictoryCenter.BLL.Interfaces;
using VictoryCenter.BLL.Services;

namespace VictoryCenter.Extensions;

public static class ServicesConfiguration
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(MediatRMarker).Assembly));
        
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        
        services.AddScoped<IPagesService, PagesService>();
    }
}
