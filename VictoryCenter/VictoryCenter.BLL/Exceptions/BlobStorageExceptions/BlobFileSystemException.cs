namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

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
