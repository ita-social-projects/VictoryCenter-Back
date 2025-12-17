using System.Text.Json;
using System.Text.Json.Serialization;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.UnitTests.HelperTests;

public class JsonValidationHelperTests
{
    private static readonly JsonSerializerOptions StrictOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        ReadCommentHandling = JsonCommentHandling.Disallow,
        AllowTrailingCommas = false,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void ValidateJsonAgainstType_WithValidJson_ShouldReturnEmptyErrors()
    {
        var json = "{\"name\":\"John\",\"age\":30}";

        var errors = JsonValidationHelper.ValidateJsonAgainstType(json, typeof(TestDto), StrictOptions);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateJsonAgainstType_WithUnmappedProperty_ShouldReturnError()
    {
        var json = "{\"name\":\"John\",\"age\":30,\"extra\":\"value\"}";

        var errors = JsonValidationHelper.ValidateJsonAgainstType(json, typeof(TestDto), StrictOptions);

        Assert.Single(errors);
        Assert.Contains("Unknown property 'extra' is not allowed", errors[0]);
    }

    [Fact]
    public void ValidateJsonAgainstType_WithMultipleUnmappedProperties_ShouldReturnAllErrors()
    {
        var json = "{\"name\":\"John\",\"age\":30,\"extra1\":\"value1\",\"extra2\":\"value2\"}";

        var errors = JsonValidationHelper.ValidateJsonAgainstType(json, typeof(TestDto), StrictOptions);

        Assert.Equal(2, errors.Count);
        Assert.All(errors, error => Assert.Contains("Unknown property", error));
    }

    [Fact]
    public void ValidateJsonAgainstType_WithCaseInsensitivePropertyNames_ShouldReturnEmptyErrors()
    {
        var json = "{\"NAME\":\"John\",\"AGE\":30}";

        var errors = JsonValidationHelper.ValidateJsonAgainstType(json, typeof(TestDto), StrictOptions);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateJsonAgainstType_WithMalformedJson_ShouldReturnError()
    {
        var json = "{\"name\":\"John\",\"age\":}";

        var errors = JsonValidationHelper.ValidateJsonAgainstType(json, typeof(TestDto), StrictOptions);

        Assert.Single(errors);
    }

    private class TestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
