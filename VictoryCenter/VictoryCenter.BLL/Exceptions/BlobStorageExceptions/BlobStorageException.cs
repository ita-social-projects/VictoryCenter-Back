namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

public abstract class BlobStorageException : Exception
{
    protected BlobStorageException(string message)
        : base(message)
    {
    }

    protected BlobStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
