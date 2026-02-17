namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

public class InvalidPdfFormatException : BlobStorageException
{
    public InvalidPdfFormatException(string message)
        : base(message)
    {
    }

    public InvalidPdfFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
