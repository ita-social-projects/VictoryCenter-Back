namespace VictoryCenter.BLL.Exceptions.BlobStorageExceptions;

/// <summary>
/// Represents errors that occur during image processing operations.
/// (e.g., saving, reading, or generating URLs for images in blob storage).
/// </summary>
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
