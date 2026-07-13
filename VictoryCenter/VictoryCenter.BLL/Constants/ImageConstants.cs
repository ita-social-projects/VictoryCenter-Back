namespace VictoryCenter.BLL.Constants;

public static class ImageConstants
{
    public static readonly int MaxImageSizeInMb = 5;
    public static readonly int BytesPerMb = 1024 * 1024;
    public static readonly int MaxImageSizeInBytes = MaxImageSizeInMb * BytesPerMb;

    public static readonly string[] AllowedMimeTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    ];

    public static readonly string Base64ValidationError = "Base64 content is invalid";
    public static readonly string FailToSaveImageInStorage = "An error occurred while saving the image in storage";
    public static readonly string FailToSaveImageInDatabase = "An error occurred while saving the image in database";
    public static readonly string InvalidBase64String = "Invalid Base64 string.";
    public static readonly string FailedToConvertBase64 = "Failed to convert Base64";
    public static readonly string ImageNotFoundGeneric = "Image not found";
    public static readonly string ImageDataNotAvailable = "Image data not available";
    public static readonly string FailedToSaveImage = "Failed to save the image.";
    public static readonly string FailedToReadImage = "Failed to retrieve the image.";
    public static readonly string HttpContextIsNotAvailable = "HttpContext is not available.";
    public static readonly string FailToCreateDirectory = "Failed to create blob storage directory";
    public static readonly string InvalidImageSize = "Invalid image size";

    public static string MimeTypeValidationError(string[] types)
    {
        return $"MimeType must be one of the following: {string.Join(", ", types)}";
    }

    public static string FileNotFound(string filePath)
    {
        return $"File not found: {filePath}";
    }

    public static string ErrorWithUserImage(string message)
    {
        return $"Error with user image: {message}";
    }

    public static string WrongFileName(string name)
    {
        return $"An error occurred while retrieving the file {name}.";
    }
}
