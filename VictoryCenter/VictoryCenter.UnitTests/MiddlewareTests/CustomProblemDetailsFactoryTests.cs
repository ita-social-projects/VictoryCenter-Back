using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using VictoryCenter.WebAPI.Factories;

namespace VictoryCenter.UnitTests.MiddlewareTests;

public class CustomProblemDetailsFactoryTests
{
    private readonly CustomProblemDetailsFactory _factory;
    private readonly DefaultHttpContext _httpContext;

    public CustomProblemDetailsFactoryTests()
    {
        var options = Options.Create(new ApiBehaviorOptions());
        _factory = new CustomProblemDetailsFactory(options);
        _httpContext = new DefaultHttpContext();
    }

    [Theory]
    [InlineData(400, "Bad Request", "One or more validation errors occurred.")]
    [InlineData(401, "Unauthorized", "Authentication is required to access this resource.")]
    [InlineData(403, "Forbidden", "You do not have permission to access this resource.")]
    [InlineData(404, "Not Found", "Resource was not found.")]
    [InlineData(409, "Conflict", "A conflict occurred with the current state of the resource.")]
    [InlineData(500, "Internal Server Error", "Please, try again.")]
    public void CreateProblemDetails_Defaults_ShouldUseDefaultValues(
        int statusCode,
        string expectedTitle,
        string expectedDetail)
    {
        // Act
        var problemDetails = _factory.CreateProblemDetails(_httpContext, statusCode);

        // Assert
        Assert.Equal(statusCode, problemDetails.Status);
        Assert.Equal(expectedTitle, problemDetails.Title);
        Assert.Equal(expectedDetail, problemDetails.Detail);
    }

    [Fact]
    public void CreateProblemDetails_Overrides_ShouldOverrideTitleAndDetail()
    {
        // Arrange
        const int code = 422;
        const string customTitle = "test title";
        const string customDetail = "test detail";

        // Act
        var problemDetails = _factory.CreateProblemDetails(
            _httpContext,
            statusCode: code,
            title: customTitle,
            detail: customDetail);

        // Assert
        Assert.Equal(code, problemDetails.Status);
        Assert.Equal(customTitle, problemDetails.Title);
        Assert.Equal(customDetail, problemDetails.Detail);
    }

    [Fact]
    public void CreateValidationProblemDetails_Defaults_And_Errors_Applied()
    {
        // Arrange
        var ms = new ModelStateDictionary();
        ms.AddModelError("FieldA", "Error A occurred");
        ms.AddModelError("FieldB", "Error B occurred");

        // Act
        var validationPD = _factory
            .CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, validationPD.Status);

        Assert.Equal("Bad Request", validationPD.Title);
        Assert.Equal(
            "One or more validation errors occurred.",
            validationPD.Detail);

        Assert.Contains("Error A occurred", validationPD.Errors["FieldA"]);
        Assert.Contains("Error B occurred", validationPD.Errors["FieldB"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_Overrides_Title_And_Detail()
    {
        // Arrange
        var ms = new ModelStateDictionary();
        ms.AddModelError("FieldA", "Error A occurred");
        ms.AddModelError("FieldB", "Error B occurred");

        const int code = 422;
        const string customTitle = "test title";
        const string customDetail = "test detail";

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(
            _httpContext,
            ms,
            statusCode: code,
            title: customTitle,
            detail: customDetail);

        // Assert
        Assert.Equal(code, validationPD.Status);
        Assert.Equal(customTitle, validationPD.Title);
        Assert.Equal(customDetail, validationPD.Detail);

        Assert.Contains("Error A occurred", validationPD.Errors["FieldA"]);
        Assert.Contains("Error B occurred", validationPD.Errors["FieldB"]);
    }
}
