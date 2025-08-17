namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

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
