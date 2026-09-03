using System.Text.Json;
using System.Text;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.UnitTests.HelperTests;

public class TrimStringJsonHelperTests
{
    private readonly JsonSerializerOptions _options;

    public TrimStringJsonHelperTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new TrimStringJsonHelper());
    }

    [Theory]
    [InlineData("\" A \"", "A")]
    [InlineData("\"  AB  \"", "AB")]
    [InlineData("\"NoSpaces\"", "NoSpaces")]
    [InlineData("\"   \"", "")]
    [InlineData("\"\"", "")]
    public void Read_TrimsLeadingAndTrailingSpaces(string json, string expected)
    {
        var result = JsonSerializer.Deserialize<string>(json, _options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(" A ", "\" A \"")]
    [InlineData("NoSpaces", "\"NoSpaces\"")]
    [InlineData("", "\"\"")]
    [InlineData(null, "null")]
    public void Write_DoesNotAlterValue(string? value, string expectedJson)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            var converter = new TrimStringJsonHelper();
            converter.Write(writer, value, _options);
            writer.Flush();
        }

        var json = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public void Read_ThrowsJsonException_WhenTokenIsNotStringOrNull()
    {
        var exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<string>("123", _options));
        Assert.Contains("Expected String or Null", exception.Message);
    }

    [Fact]
    public void Read_ReturnsEmptyString_WhenJsonValueIsNull()
    {
        var result = JsonSerializer.Deserialize<string>("null", _options);
        Assert.Equal(string.Empty, result);
    }
}
