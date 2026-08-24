using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.UnitTests.HelperTests;

public class HtmlContentHelperTests
{
    [Fact]
    public void StripHtmlTags_NullInput_ShouldReturnEmptyString()
    {
        // Act
        var result = HtmlContentHelper.StripHtmlTags(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void StripHtmlTags_EmptyInput_ShouldReturnEmptyString()
    {
        // Act
        var result = HtmlContentHelper.StripHtmlTags(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void StripHtmlTags_PlainText_ShouldReturnSameText()
    {
        // Arrange
        const string input = "Plain text without markup";

        // Act
        var result = HtmlContentHelper.StripHtmlTags(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void StripHtmlTags_TextWithHtmlTags_ShouldReturnOnlyVisibleText()
    {
        // Arrange
        const string input = "<p><strong>Bold</strong> and <em>italic</em> text</p>";

        // Act
        var result = HtmlContentHelper.StripHtmlTags(input);

        // Assert
        Assert.Equal("Bold and italic text", result);
    }

    [Fact]
    public void StripHtmlTags_TextWithLineBreaks_ShouldStripTagsAndKeepVisibleLength()
    {
        // Arrange
        const string input = "Line one<br />Line two<br /><b>Line three</b>";

        // Act
        var result = HtmlContentHelper.StripHtmlTags(input);

        // Assert
        Assert.Equal("Line oneLine twoLine three", result);
    }
}
