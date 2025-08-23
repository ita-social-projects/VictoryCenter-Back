namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

/// <summary>
/// Represents errors that occur when working with the file system
/// during blob storage operations (e.g., creating, deleting, or reading files).
/// </summary>
public class BlobFileSystemException : BlobStorageException
{
    public BlobFileSystemException(string path, string message)
        : base(message)
    {
        Path = path;
    }

    public BlobFileSystemException(string path, string message, Exception innerException)
        : base(message, innerException)
    {
        Path = path;
    }

    public string Path { get; }
}
