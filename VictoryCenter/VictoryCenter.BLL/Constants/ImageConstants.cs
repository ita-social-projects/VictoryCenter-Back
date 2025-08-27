namespace VictoryCenter.BLL.Constants;

public static class ImageConstants
{
    public static readonly string Base64ValidationError = "Base64 content is invalid";
    public static readonly string FailToSaveImageInStorage = "An error occurred while saving the image in storage";
    public static readonly string FailToSaveImageInDatabase = "An error occurred while saving the image in database";
    public static readonly string InvalidBase64String = "Invalid Base64 string.";
    public static readonly string FailedToConvertBase64 = "Failed to convert Base64";
    public static readonly string InvalidIVLength = "Invalid IV length";
    public static readonly string ImageNotFoundGeneric = "Image not found";
    public static readonly string EncryptionFailed = "Encryption failed.";
    public static readonly string DecryptionFailed = "Decryption failed.";
    public static readonly string ImageDataNotAvailable = "Image data not available";

    public static string MimeTypeValidationError(string[] types)
    {
        return $"MimeType must be one of the following: {string.Join(", ", types)}";
    }

    public static string FileNotFound(string filePath)
    {
        return $"File not found: {filePath}";
    }
}
