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
}
