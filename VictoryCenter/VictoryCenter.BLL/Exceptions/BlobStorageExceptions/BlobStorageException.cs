namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

/// <summary>
/// Serves as the base class for all blob storage-related exceptions.
/// </summary>
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
