using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.WebAPI.Filters;

public sealed class ContentLockOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.MethodInfo.GetCustomAttributes(true).OfType<RequireContentLockAttribute>().Any())
        {
            return;
        }

        operation.Parameters ??= [];
        AddHeader(
            operation,
            ContentLockConstants.LockIdHeader,
            "Opaque lock id returned by the content-lock hub. Required when ContentLock:EnforcementEnabled is true.");
        AddHeader(
            operation,
            ContentLockConstants.ConnectionIdHeader,
            "Active SignalR connection id that owns the lock. Required when ContentLock:EnforcementEnabled is true.");
    }

    private static void AddHeader(OpenApiOperation operation, string name, string description)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = name,
            Description = description,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}
