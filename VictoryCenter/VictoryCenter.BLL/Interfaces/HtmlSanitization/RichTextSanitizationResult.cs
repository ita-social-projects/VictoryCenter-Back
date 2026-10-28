namespace VictoryCenter.BLL.Interfaces.HtmlSanitization;

// RemovedItems stays empty when the input only differed in serialization (e.g. "<br/>" vs "<br>")
public sealed record RichTextSanitizationResult(string Html, IReadOnlyCollection<string> RemovedItems)
{
    public bool HasRemovedContent => RemovedItems.Count > 0;
}
