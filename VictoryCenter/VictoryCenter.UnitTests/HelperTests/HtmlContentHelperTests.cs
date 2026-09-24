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

    [Fact]
    public void GetVisibleText_TextWithHtmlEntity_ShouldDecodeEntity()
    {
        // Act
        var result = HtmlContentHelper.GetVisibleText("<p>abcde&amp;</p>");

        // Assert
        Assert.Equal("abcde&", result);
    }

    [Theory]
    [InlineData("   Valid text   ", "Valid text")]
    [InlineData("<p>   Valid text   </p>", "<p>Valid text</p>")]
    [InlineData("<p><strong>   Valid text   </strong></p>", "<p><strong>Valid text</strong></p>")]
    [InlineData("<p>   Valid&amp;text   </p>", "<p>Valid&amp;text</p>")]
    [InlineData("&nbsp;Valid text&nbsp;", "Valid text")]
    [InlineData("<p>Valid  text</p>", "<p>Valid  text</p>")]
    public void NormalizeHtmlContent_TextWithWhitespace_ShouldTrimOnlyVisibleTextEdges(string input, string expected)
    {
        // Act
        var result = HtmlContentHelper.NormalizeHtmlContent(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
