using Microsoft.AspNetCore.Http;

namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

/// <summary>
/// Represents errors that occur when working with <see cref="HttpContext"/>.
/// during blob storage operations (e.g., when building URLs).
/// </summary>
public class BlobHttpContextException : BlobStorageException
{
    public BlobHttpContextException(string message)
        : base(message)
    {
    }

    public BlobHttpContextException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
