namespace VictoryCenter.BLL.Constants;

public static class ImageConstants
{
    public static readonly string Base64ValidationError = "Base64 content is invalid";
    public static readonly string CreateImageDtoCantBeNull = "CreateImageDto cannot be null";
    public static readonly string UpdateImageDtoCantBeNull = "UpdateImageDto cannot be null";
    public static readonly string FailToSaveImageInStorage = "An error occurred while saving the image in storage";
    public static readonly string FailToSaveImageInDatabase = "An error occurred while saving the image in database";
    public static readonly string FailToDeleteImage = "Failed to delete image.";
    public static readonly string FailToUpdateImage = "Failed to update image";
    public static readonly string InvalidBase64String = "Invalid Base64 string.";
    public static readonly string FailedToConvertBase64 = "Failed to convert Base64";
    public static readonly string InvalidIVLength = "Invalid IV length";
    public static readonly string ImageNotFoundGeneric = "Image not found";
    public static readonly string ImageBlobNameIsNull = "Image blob name is null";
    public static readonly string UnexpectedBlobReadError = "Unexpected error during file retrieval.";
    public static readonly string ImageDataNotAvailable = "Image data not available";
    public static readonly string FailedToSaveImage = "Failed to save the image.";
    public static readonly string FailedToReadImage = "Failed to retrieve the image.";

    public static string FieldIsRequired(string name)
    {
        return $"{name} is required";
    }

    public static string MimeTypeValidationError(string[] types)
    {
        return $"MimeType must be one of the following: {string.Join(", ", types)}";
    }

    public static string FileNotFound(string filePath)
    {
        return $"File not found: {filePath}";
    }

    public static string CantGetFile(string name)
    {
        return $"An error occurred while retrieving the file {name}.";
    }
}
