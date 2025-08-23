namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

/// <summary>
/// Represents errors that occur when an invalid file name is provided.
/// for blob storage operations.
/// </summary>
public class BlobFileNameException : BlobStorageException
{
    public BlobFileNameException(string name, string message)
        : base(message)
    {
        Name = name;
    }

    public BlobFileNameException(string name, string message, Exception innerException)
        : base(message, innerException)
    {
        Name = name;
    }

    public string Name { get; }
}
