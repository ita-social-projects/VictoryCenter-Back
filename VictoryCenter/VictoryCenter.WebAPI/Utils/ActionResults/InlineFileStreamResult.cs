using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace VictoryCenter.WebAPI.Utils.ActionResults;

public class InlineFileStreamResult : FileStreamResult
{
    private readonly string _fileName;

    public InlineFileStreamResult(Stream fileStream, string contentType, string fileName)
        : base(fileStream, contentType)
    {
        _fileName = fileName;
    }

    public override Task ExecuteResultAsync(ActionContext context)
    {
        var contentDisposition = new ContentDispositionHeaderValue("inline")
        {
            FileNameStar = _fileName
        };

        context.HttpContext.Response.Headers.Append(HeaderNames.ContentDisposition, contentDisposition.ToString());

        context.HttpContext.Response.Headers.Append(HeaderNames.CacheControl, "private, no-store");

        return base.ExecuteResultAsync(context);
    }
}
