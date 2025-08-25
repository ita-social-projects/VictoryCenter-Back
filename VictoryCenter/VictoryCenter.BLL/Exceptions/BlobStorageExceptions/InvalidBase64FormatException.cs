namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

/// <summary>
/// Represents errors that occur when a provided Base64 string is invalid or cannot be converted to bytes.
/// </summary>
public class InvalidBase64FormatException : BlobStorageException
{
    public InvalidBase64FormatException(string message)
        : base(message)
    {
    }

    public InvalidBase64FormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
