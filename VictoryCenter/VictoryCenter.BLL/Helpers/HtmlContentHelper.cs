using System.Net;
using HtmlAgilityPack;

namespace VictoryCenter.BLL.Helpers;

public static class HtmlContentHelper
{
    public static string StripHtmlTags(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(input);
        return htmlDoc.DocumentNode.InnerText;
    }

    public static string GetVisibleText(string? input)
    {
        return HtmlEntity.DeEntitize(StripHtmlTags(input)).Trim();
    }

    public static string NormalizeHtmlContent(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(input);

        if (htmlDoc.DocumentNode.SelectNodes("//*") == null)
        {
            return input.Trim();
        }

        var textNodes = htmlDoc.DocumentNode.SelectNodes("//text()")?.OfType<HtmlTextNode>().ToList();
        if (textNodes == null || textNodes.Count == 0)
        {
            return input.Trim();
        }

        TrimStart(textNodes[0]);
        TrimEnd(textNodes[^1]);

        return htmlDoc.DocumentNode.InnerHtml;
    }

    private static void TrimStart(HtmlTextNode textNode)
    {
        var decodedText = HtmlEntity.DeEntitize(textNode.Text);
        var trimmedText = decodedText.TrimStart();

        if (decodedText != trimmedText)
        {
            textNode.Text = WebUtility.HtmlEncode(trimmedText);
        }
    }

    private static void TrimEnd(HtmlTextNode textNode)
    {
        var decodedText = HtmlEntity.DeEntitize(textNode.Text);
        var trimmedText = decodedText.TrimEnd();

        if (decodedText != trimmedText)
        {
            textNode.Text = WebUtility.HtmlEncode(trimmedText);
        }
    }
}
