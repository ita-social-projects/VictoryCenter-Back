using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VictoryCenter.WebAPI.Filters;

public class StrictJsonValidationFilter : IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        if (context.HttpContext.Request.ContentType?.Contains("application/json") == true &&
            context.HttpContext.Request.ContentLength > 0)
        {
            context.HttpContext.Request.EnableBuffering();

            using var reader = new StreamReader(
                context.HttpContext.Request.Body,
                Encoding.UTF8,
                leaveOpen: true);

            var json = await reader.ReadToEndAsync();
            context.HttpContext.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(json))
            {
                var endpoint = context.HttpContext.GetEndpoint();
                var actionDescriptor = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

                if (actionDescriptor != null)
                {
                    foreach (var parameter in actionDescriptor.Parameters)
                    {
                        if (parameter.ParameterType.IsClass &&
                            parameter.ParameterType != typeof(string) &&
                            !parameter.ParameterType.IsAbstract)
                        {
                            var options = new JsonSerializerOptions
                            {
                                UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow,
                                ReadCommentHandling = JsonCommentHandling.Disallow,
                                AllowTrailingCommas = false,
                                PropertyNameCaseInsensitive = true
                            };

                            try
                            {
                                JsonSerializer.Deserialize(json, parameter.ParameterType, options);
                            }
                            catch (JsonException ex)
                            {
                                var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
                                modelState.AddModelError(string.Empty, $"Invalid JSON: {ex.Message}");
                                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState));
                                return;
                            }
                        }
                    }
                }
            }
        }

        await next();
    }
}
