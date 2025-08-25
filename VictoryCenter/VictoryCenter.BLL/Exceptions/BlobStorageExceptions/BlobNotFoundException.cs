namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

/// <summary>
/// Represents errors that occur when a requested file is not found in the blob storage.
/// </summary>
public class BlobNotFoundException : BlobStorageException
{
    public BlobNotFoundException(string fileName, string message)
        : base(message)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
