using AngleSharp.Css.Dom;
using Ganss.Xss;
using VictoryCenter.BLL.Interfaces.HtmlSanitization;

namespace VictoryCenter.BLL.Services.HtmlSanitization;

public class RichTextSanitizer : IRichTextSanitizer
{
    // Other disallowed tags are unwrapped and keep their text; these are dropped together with their content
    internal static readonly IReadOnlySet<string> TagsRemovedWithContent = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "script", "style", "iframe", "frame", "frameset", "object", "embed", "applet", "noscript", "noembed",
        "noframes", "template", "textarea", "select", "svg", "math", "title", "head", "xmp", "plaintext",
    };

    // Matches the output of the admin rich-text editor: paragraphs, bold, italic and line breaks
    private static readonly HtmlSanitizerOptions Options = new()
    {
        AllowedTags = new HashSet<string>(["p", "strong", "em", "b", "i", "br"], StringComparer.OrdinalIgnoreCase),
        AllowedAttributes = new HashSet<string>(),
        AllowedCssClasses = new HashSet<string>(),
        AllowedCssProperties = new HashSet<string>(),
        AllowedAtRules = new HashSet<CssRuleType>(),
        AllowedSchemes = new HashSet<string>(),
        UriAttributes = new HashSet<string>(),
        UriListAttributes = new HashSet<string>(),
    };

    public RichTextSanitizationResult Sanitize(string html)
    {
        var removedItems = new List<string>();

        // A new instance per call because the event handlers collect per-call state
        var sanitizer = new HtmlSanitizer(Options) { KeepChildNodes = true };

        sanitizer.RemovingTag += (_, e) =>
        {
            // Already removed together with a dropped ancestor
            if (e.Tag.Owner?.Contains(e.Tag) != true)
            {
                e.Cancel = true;
                return;
            }

            removedItems.Add($"<{e.Tag.LocalName}>");

            if (TagsRemovedWithContent.Contains(e.Tag.LocalName))
            {
                e.Cancel = true;
                e.Tag.Remove();
            }
        };
        sanitizer.RemovingAttribute += (_, e) => removedItems.Add($"{e.Attribute.Name} attribute");
        sanitizer.RemovingComment += (_, _) => removedItems.Add("comment");

        var sanitizedHtml = sanitizer.Sanitize(html);

        return new RichTextSanitizationResult(sanitizedHtml, removedItems.Distinct().ToList());
    }
}
