using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
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

    [Fact]
    public void CreateValidationProblemDetails_NonNumericAmount_ReturnsOnlyDigitsErrorWithoutBodyRequired()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("dto", "The dto field is required.");
        ms.AddModelError(
            "$.amount",
            "The JSON value could not be converted to System.Decimal. Path: $.amount | LineNumber: 2 | BytePositionInLine: 20.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        var error = Assert.Single(validationPD.Errors);
        Assert.Equal("UpdateTestDto.Amount", error.Key);
        Assert.Equal(new[] { ErrorMessagesConstants.PropertyMustContainOnlyDigits("Amount") }, error.Value);
    }

    [Fact]
    public void CreateValidationProblemDetails_FrameworkMessagesDisabled_StillDetectsNumericProperty()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("$.amount", "The input was not valid.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(
            new[] { ErrorMessagesConstants.PropertyMustContainOnlyDigits("Amount") },
            validationPD.Errors["UpdateTestDto.Amount"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_NonNumericProperty_ReturnsInvalidFormatError()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError(
            "$.reportingDate",
            "The JSON value could not be converted to System.DateTime. Path: $.reportingDate | LineNumber: 2 | BytePositionInLine: 25.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(
            new[] { ErrorMessagesConstants.PropertyMustBeInAValidFormat("ReportingDate") },
            validationPD.Errors["UpdateTestDto.ReportingDate"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_NumericCollection_ReturnsInvalidFormatError()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError(
            "$.ids",
            "The JSON value could not be converted to System.Collections.Generic.List`1[System.Int32]. Path: $.ids | LineNumber: 2 | BytePositionInLine: 15.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(
            new[] { ErrorMessagesConstants.PropertyMustBeInAValidFormat("Ids") },
            validationPD.Errors["UpdateTestDto.Ids"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_CollectionElement_ReturnsOnlyDigitsError()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("$.ids[0]", "The input was not valid.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(
            new[] { ErrorMessagesConstants.PropertyMustContainOnlyDigits("Ids[0]") },
            validationPD.Errors["UpdateTestDto.Ids[0]"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_JsonPropertyNameAttribute_ResolvesPropertyType()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("$.sum", "The input was not valid.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(
            new[] { ErrorMessagesConstants.PropertyMustContainOnlyDigits("Sum") },
            validationPD.Errors["UpdateTestDto.Sum"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_NestedProperty_ResolvesPropertyType()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("$.details.count", "The input was not valid.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(
            new[] { ErrorMessagesConstants.PropertyMustContainOnlyDigits("Details.Count") },
            validationPD.Errors["UpdateTestDto.Details.Count"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_UnknownProperty_ReturnsInvalidFormatError()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("$.unknown", "The input was not valid.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        Assert.Equal(
            new[] { ErrorMessagesConstants.PropertyMustBeInAValidFormat("Unknown") },
            validationPD.Errors["UpdateTestDto.Unknown"]);
    }

    [Fact]
    public void CreateValidationProblemDetails_RootJsonError_ReturnsBodyLevelError()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("dto", "The dto field is required.");
        ms.AddModelError("$", "'a' is an invalid start of a value. Path: $ | LineNumber: 0 | BytePositionInLine: 0.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        var error = Assert.Single(validationPD.Errors);
        Assert.Equal("UpdateTestDto", error.Key);
        Assert.Equal(new[] { ErrorMessagesConstants.PropertyMustBeInAValidFormat("UpdateTestDto") }, error.Value);
    }

    [Fact]
    public void CreateValidationProblemDetails_EmptyJsonPathSegment_ReturnsBodyLevelErrorWithoutException()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("$.", "The input was not valid.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        var error = Assert.Single(validationPD.Errors);
        Assert.Equal("UpdateTestDto", error.Key);
    }

    [Fact]
    public void CreateValidationProblemDetails_SeveralRootErrors_MergesMessagesUnderOneKey()
    {
        // Arrange
        SetBodyParameter("dto");
        var ms = new ModelStateDictionary();
        ms.AddModelError("$", "Root error.");
        ms.AddModelError("$[0]", "Array root error.");

        // Act
        var validationPD = _factory.CreateValidationProblemDetails(_httpContext, ms);

        // Assert
        var error = Assert.Single(validationPD.Errors);
        Assert.Equal("UpdateTestDto", error.Key);
        Assert.Single(error.Value);
    }

    private void SetBodyParameter(string name)
    {
        var actionDescriptor = new ControllerActionDescriptor
        {
            Parameters =
            [
                new ParameterDescriptor
                {
                    Name = name,
                    ParameterType = typeof(UpdateTestDto),
                    BindingInfo = new BindingInfo { BindingSource = BindingSource.Body },
                },
            ],
        };
        _httpContext.SetEndpoint(new Endpoint(null, new EndpointMetadataCollection(actionDescriptor), "test"));
    }

    private sealed class UpdateTestDto
    {
        public decimal? Amount { get; set; }

        public DateTime ReportingDate { get; set; }

        public List<int> Ids { get; set; } = [];

        [JsonPropertyName("sum")]
        public decimal Total { get; set; }

        public NestedTestDto Details { get; set; } = new();
    }

    private sealed class NestedTestDto
    {
        public int Count { get; set; }
    }
}
