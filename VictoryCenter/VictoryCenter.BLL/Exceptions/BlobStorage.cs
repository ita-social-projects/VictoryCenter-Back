namespace VictoryCenter.BLL.Exceptions;

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

public class BlobNotFoundException : BlobStorageException
{
    public BlobNotFoundException(string fileName, string message)
        : base(message)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}

public class ImageProcessingException : BlobStorageException
{
    public ImageProcessingException(string fileName, string message)
        : base(message)
    {
        FileName = fileName;
    }

    public ImageProcessingException(string fileName, string message, Exception innerException)
        : base(message, innerException)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}

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
