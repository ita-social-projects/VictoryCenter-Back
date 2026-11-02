namespace VictoryCenter.BLL.Interfaces.HtmlSanitization;

public interface IRichTextSanitizer
{
    RichTextSanitizationResult Sanitize(string html);
}
