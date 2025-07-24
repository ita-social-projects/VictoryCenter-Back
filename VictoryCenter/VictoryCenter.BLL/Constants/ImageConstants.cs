namespace VictoryCenter.BLL.Constants;

public static class ImageConstants
{
    public static readonly string Base64ValidationError = "Base64 content is invalid";
    public static readonly string CreateImageDtoCantBeNull = "CreateImageDto cannot be null";
    public static readonly string FailToSaveImageInStorage = "An error occurred while saving the image in storage";
    public static readonly string FailToSaveImageInDatabase = "An error occurred while saving the image in database";
    public static readonly string FailToDeleteImage = "Failed to delete image.";
    public static readonly string FailToUpdateImage = "Failed to update image";
    public static readonly string InvalidBase64String = "Invalid Base64 string.";
    public static readonly string InvalidIVLength = "Invalid IV length";
    public static readonly string ImageNotFoundGeneric = "Image not found";
    public static readonly string ImageBlobNameIsNull = "Image blob name is null";
    public static readonly string EncryptionFailed = "Encryption failed.";
    public static readonly string DecryptionFailed = "Decryption failed.";
    public static readonly string FailedToWriteEncryptedFile = "Failed to write encrypted file.";
    public static readonly string FailedToReadOrDecryptFile = "Failed to read or decrypt file.";
    public static readonly string UnexpectedBlobReadError = "Unexpected error during file retrieval.";

    public static string FieldIsRequired(string name)
    {
        return $"{name} is required";
    }

    public static string MimeTypeValidationError(string[] types)
    {
        return $"MimeType must be one of the following: {string.Join(", ", types)}";
    }

    public static string ImageNotFound(long id)
    {
        return $"Image with ID {id} not found.";
    }

    public static string FileNotFound(string filePath)
    {
        return $"File not found: {filePath}";
    }
}
