namespace VictoryCenter.BLL.Constants;

public static class ImageConstants
{
    public static readonly string Base64ValidationError = "Base64 content is invalid";
    public static readonly string CreateImageDtoCantBeNull = "public static readonly string CreateImageDtoCantBeNull";
    public static readonly string FailToSaveImageInStorage = "An error occurred while saving the photo in storage";
    public static readonly string FailToSaveImageInDatabase = "An error occurred while saving the photo in database";
    public static string FieldIsRequired(string name)
    {
        return $"{name} is required";
    }

    public static string MimeTypeValidationError(string[] types)
    {
        return $"MimeType must be one of the following: {string.Join(", ", types)}";
    }
}
