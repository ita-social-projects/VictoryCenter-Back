namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

public class BlobNotFoundException : BlobStorageException
{
    public BlobNotFoundException(string fileName, string message)
        : base(message)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
