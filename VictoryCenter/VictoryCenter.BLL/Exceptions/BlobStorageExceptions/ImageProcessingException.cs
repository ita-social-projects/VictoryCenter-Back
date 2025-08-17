namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

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
