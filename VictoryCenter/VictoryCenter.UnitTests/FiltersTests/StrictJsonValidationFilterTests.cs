using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using VictoryCenter.WebAPI.Filters;

namespace VictoryCenter.UnitTests.FiltersTests;

public class StrictJsonValidationFilterTests
{
    private readonly StrictJsonValidationFilter _filter = new();

    [Theory]
    [InlineData(null)]
    [InlineData("text/plain")]
    [InlineData("application/xml")]
    public async Task OnResourceExecutionAsync_WithNonJsonContentType_ShouldCallNext(string? contentType)
    {
        var (context, nextCalled) = await ExecuteFilter(contentType);

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task OnResourceExecutionAsync_WithEmptyBody_ShouldCallNext(string body)
    {
        var (context, nextCalled) = await ExecuteFilter("application/json", body);

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnResourceExecutionAsync_WithNoActionDescriptor_ShouldCallNext()
    {
        var context = CreateContext("application/json", "{\"name\":\"test\"}");
        context.HttpContext.SetEndpoint(new Endpoint(null, new EndpointMetadataCollection(), null));
        var nextCalled = false;

        await _filter.OnResourceExecutionAsync(context, () =>
        {
            nextCalled = true;
            return Task.FromResult(CreateExecutedContext(context));
        });

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Theory]
    [InlineData("{\"name\":\"John\",\"age\":30}")]
    [InlineData("{\"NAME\":\"John\",\"AGE\":30}")]
    public async Task OnResourceExecutionAsync_WithValidJson_ShouldCallNext(string json)
    {
        var (context, nextCalled) = await ExecuteFilter("application/json", json, typeof(TestDto));

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Theory]
    [InlineData("{\"name\":\"John\",\"age\":30,\"extra\":\"field\"}")]
    [InlineData("{\"name\":\"John\",\"age\":}")]
    [InlineData("{\"name\":\"John\", /* comment */ \"age\":30}")]
    [InlineData("{\"name\":\"John\",\"age\":30,}")]
    public async Task OnResourceExecutionAsync_WithInvalidJson_ShouldReturnBadRequest(string json)
    {
        var (context, nextCalled) = await ExecuteFilter("application/json", json, typeof(TestDto));

        Assert.False(nextCalled);
        var badRequest = Assert.IsType<BadRequestObjectResult>(context.Result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        Assert.Contains("Invalid JSON:", problemDetails.Errors[string.Empty][0]);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json; charset=utf-8")]
    [InlineData("APPLICATION/JSON")]
    public async Task OnResourceExecutionAsync_WithVariousJsonContentTypes_ShouldValidate(string contentType)
    {
        var (context, _) = await ExecuteFilter(contentType, "{\"name\":\"John\",\"extra\":\"field\"}", typeof(TestDto));

        Assert.IsType<BadRequestObjectResult>(context.Result);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(AbstractTestClass))]
    public async Task OnResourceExecutionAsync_WithNonValidatableParameters_ShouldSkipValidation(Type parameterType)
    {
        var (context, nextCalled) = await ExecuteFilter("application/json", "{\"invalid json}", parameterType);

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnResourceExecutionAsync_RequestBodyCanBeReadMultipleTimes()
    {
        var json = "{\"name\":\"John\",\"age\":30}";
        var context = CreateContext("application/json", json, typeof(TestDto));

        await _filter.OnResourceExecutionAsync(context, () =>
        {
            context.HttpContext.Request.Body.Position = 0;
            using var reader = new StreamReader(context.HttpContext.Request.Body);
            var bodyContent = reader.ReadToEnd();
            Assert.Equal(json, bodyContent);
            return Task.FromResult(CreateExecutedContext(context));
        });

        Assert.Null(context.Result);
    }

    private async Task<(ResourceExecutingContext context, bool nextCalled)> ExecuteFilter(
        string? contentType,
        string? body = null,
        params Type[] parameterTypes)
    {
        var context = CreateContext(contentType, body, parameterTypes);
        var nextCalled = false;

        await _filter.OnResourceExecutionAsync(context, () =>
        {
            nextCalled = true;
            return Task.FromResult(CreateExecutedContext(context));
        });

        return (context, nextCalled);
    }

    private static ResourceExecutingContext CreateContext(
        string? contentType,
        string? body = null,
        params Type[] parameterTypes)
    {
        var httpContext = new DefaultHttpContext
        {
            Request = { ContentType = contentType }
        };

        if (body != null)
        {
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var parameters = parameterTypes.Select(type => new ParameterDescriptor
        {
            Name = type.Name.ToLower(),
            ParameterType = type
        }).ToList();

        httpContext.SetEndpoint(new Endpoint(
            null,
            new EndpointMetadataCollection(new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor
            {
                Parameters = parameters
            }),
            null));

        return new ResourceExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new List<IValueProviderFactory>());
    }

    private static ResourceExecutedContext CreateExecutedContext(ResourceExecutingContext executingContext) =>
        new(executingContext, new List<IFilterMetadata>()) { Result = executingContext.Result };

    private class TestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private abstract class AbstractTestClass
    {
        public string Name { get; set; } = string.Empty;
    }
}
