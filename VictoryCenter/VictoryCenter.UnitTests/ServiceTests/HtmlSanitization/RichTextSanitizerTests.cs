using VictoryCenter.BLL.Services.HtmlSanitization;

namespace VictoryCenter.UnitTests.ServiceTests.HtmlSanitization;

public class RichTextSanitizerTests
{
    private readonly RichTextSanitizer _sanitizer = new();

    [Theory]
    [InlineData("<p>Plain paragraph</p>")]
    [InlineData("<p>Hello <strong>bold</strong> and <em>italic</em><br>next line</p>")]
    [InlineData("<p><b>bold</b> and <i>italic</i></p><p>Second paragraph</p>")]
    [InlineData("<p>Привіт, <strong>світ</strong></p>")]
    [InlineData("<p>a&nbsp;&nbsp;b</p>")]
    [InlineData("<p>Tom &amp; Jerry &lt;3 \"quoted\" 'single'</p>")]
    [InlineData("<p>&lt;script&gt;alert(1)&lt;/script&gt;</p>")]
    [InlineData("Plain text without markup")]
    public void Sanitize_SupportedFormatting_ShouldRemainUnchanged(string input)
    {
        // Act
        var result = _sanitizer.Sanitize(input);

        // Assert
        Assert.Equal(input, result.Html);
        Assert.False(result.HasRemovedContent);
    }

    [Theory]
    [InlineData("<p>Text<script>alert(1)</script>after</p>", "<p>Textafter</p>")]
    [InlineData("<p>x<img src=x onerror=alert(1)>y</p>", "<p>xy</p>")]
    [InlineData("<p onclick=\"alert(1)\">x</p>", "<p>x</p>")]
    [InlineData("<p style=\"color:red\" class=\"c\">x</p>", "<p>x</p>")]
    [InlineData("<a href=\"javascript:alert(1)\">click</a>", "click")]
    [InlineData("<a href=\"https://example.com\">link</a>", "link")]
    [InlineData("<p>x<iframe src=\"https://evil.example\">inner</iframe>y</p>", "<p>xy</p>")]
    [InlineData("<svg onload=alert(1)><circle/></svg>ok", "ok")]
    [InlineData("<style>p{color:red}</style><p>x</p>", "<p>x</p>")]
    [InlineData("<!-- comment --><p>x</p>", "<p>x</p>")]
    [InlineData("<math><mtext><table><mglyph><style><img src=x onerror=alert(1)>", "")]
    public void Sanitize_UnsafeMarkup_ShouldBeRemoved(string input, string expected)
    {
        // Act
        var result = _sanitizer.Sanitize(input);

        // Assert
        Assert.Equal(expected, result.Html);
        Assert.True(result.HasRemovedContent);
    }

    [Theory]
    [InlineData("<p><span style=\"color:red\">red</span> text</p>", "<p>red text</p>")]
    [InlineData("<div><p>nested</p></div>", "<p>nested</p>")]
    [InlineData("<h1>Heading</h1>", "Heading")]
    public void Sanitize_UnsupportedWrapperTags_ShouldBeUnwrappedKeepingText(string input, string expected)
    {
        // Act
        var result = _sanitizer.Sanitize(input);

        // Assert
        Assert.Equal(expected, result.Html);
        Assert.True(result.HasRemovedContent);
    }

    [Theory]
    [InlineData("<p>a<br/>b</p>", "<p>a<br>b</p>")]
    [InlineData("<p>a&#160;b</p>", "<p>a&nbsp;b</p>")]
    [InlineData("<p>a b</p>", "<p>a&nbsp;b</p>")]
    [InlineData("<P>Upper case</P>", "<p>Upper case</p>")]
    public void Sanitize_EquivalentSerialization_ShouldNormalizeWithoutReportingRemovals(string input, string expected)
    {
        // Act
        var result = _sanitizer.Sanitize(input);

        // Assert
        Assert.Equal(expected, result.Html);
        Assert.False(result.HasRemovedContent);
    }

    [Theory]
    [InlineData("<p>Hello <strong>bold</strong><br>a&nbsp;&nbsp;b &amp; c</p>")]
    [InlineData("<p>Text<script>alert(1)</script><img src=x onerror=alert(1)><span>kept</span></p>")]
    [InlineData("<math><mtext><table><mglyph><style><img src=x onerror=alert(1)>")]
    [InlineData("<p>a&#160;b<br/></p>")]
    public void Sanitize_AlreadySanitizedOutput_ShouldNotChangeOnSecondPass(string input)
    {
        // Arrange
        var once = _sanitizer.Sanitize(input).Html;

        // Act
        var twice = _sanitizer.Sanitize(once);

        // Assert
        Assert.Equal(once, twice.Html);
        Assert.False(twice.HasRemovedContent);
    }

    [Fact]
    public void Sanitize_UnsafeMarkup_ShouldReportRemovedItems()
    {
        // Act
        var result = _sanitizer.Sanitize("<p onclick=\"x()\">a<script>b</script><!-- c --></p>");

        // Assert
        Assert.Contains("<script>", result.RemovedItems);
        Assert.Contains("onclick attribute", result.RemovedItems);
        Assert.Contains("comment", result.RemovedItems);
    }

    [Fact]
    public void Sanitize_TagRemovedWithContent_ShouldNotReportItsDescendants()
    {
        // Act
        var result = _sanitizer.Sanitize("<p>a<svg><circle onload=\"x()\"/><path/></svg></p>");

        // Assert
        Assert.Equal("<p>a</p>", result.Html);
        Assert.Equal(["<svg>"], result.RemovedItems);
    }
}
