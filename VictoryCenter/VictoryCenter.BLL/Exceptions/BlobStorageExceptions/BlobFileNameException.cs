namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

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
