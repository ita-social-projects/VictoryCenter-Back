using System.Text.Json;
using System.Text;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

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
        var json = """{"DisclaimerTitle":123}""";
        var act = () => JsonSerializer.Deserialize<UpdateReportFundsExpendituresSettingsDto>(json);
        var exception = Assert.Throws<JsonException>(act);
        Assert.Contains("Expected String or Null", exception.Message);
    }

    [Fact]
    public void Read_ReturnsEmptyString_WhenJsonValueIsNull()
    {
        var json = """{"DisclaimerTitle":null}""";
        var dto = JsonSerializer.Deserialize<UpdateReportFundsExpendituresSettingsDto>(json);
        Assert.Equal(string.Empty, dto?.DisclaimerTitle);
    }
}
