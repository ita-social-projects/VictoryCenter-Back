using System.Collections.ObjectModel;

namespace VictoryCenter.BLL.Constants;

public static class ImageConstants
{
    // The upload rules define a 5 MB encoded-file limit. The remaining limits are
    // defense-in-depth bounds for decoding untrusted content and should be reviewed
    // if the product starts accepting unusually large source images.
    public const int MaxImageSizeInMb = 5;
    public const int BytesPerMb = 1024 * 1024;
    public const int MaxImageSizeInBytes = MaxImageSizeInMb * BytesPerMb;

    // Bound both individual axes and total canvas area. The pixel-count limit is the
    // primary decompression-bomb guard; axis limits also reject pathological images.
    public const int MaxImageWidth = 10_000;
    public const int MaxImageHeight = 10_000;
    public const long MaxImagePixelCount = 25_000_000;

    // Keep decoded pixel memory and ImageSharp's internal allocations bounded even
    // when a small compressed payload declares a very large image.
    public const int MaxDecodedImageSizeInMb = 100;
    public const long MaxDecodedImageSizeInBytes = MaxDecodedImageSizeInMb * BytesPerMb;
    public const int ImageDecoderAllocationLimitInMb = 128;
    public const int ImageDecoderPoolSizeInMb = 64;

    // A 5 MB binary file expands to about 6.67 MB as Base64. Seven MB permits the
    // required JSON envelope while Kestrel rejects unexpectedly large requests early.
    public const int MaxImageUploadRequestSizeInMb = 7;
    public const long MaxImageUploadRequestSizeInBytes = MaxImageUploadRequestSizeInMb * BytesPerMb;
    public const int MaxBase64Length =
        (MaxImageSizeInBytes + Base64SourceBlockSize - 1) / Base64SourceBlockSize * Base64EncodedBlockSize;

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
    public static readonly string InvalidImageContent = "Decoded content is not a valid image";
    public static readonly string UnsupportedImageContentFormat = "The actual image format is not supported";
    public static readonly string ImageMimeTypeMismatch = "MimeType does not match the actual image format";
    public static readonly string ImageDimensionsExceeded =
        $"Image dimensions cannot exceed {MaxImageWidth}x{MaxImageHeight} pixels";
    public static readonly string ImagePixelCountExceeded =
        $"Image cannot exceed {MaxImagePixelCount} pixels";
    public static readonly string DecodedImageSizeExceeded = "Decoded image requires too much memory";
    public static readonly string AnimatedImageNotSupported = "Animated images are not supported";

    private const int Base64SourceBlockSize = 3;
    private const int Base64EncodedBlockSize = 4;

    public static ReadOnlyCollection<string> AllowedMimeTypes { get; } = Array.AsReadOnly(
    [
        ImageMimeTypes.Jpeg,
        ImageMimeTypes.Jpg,
        ImageMimeTypes.Png,
        ImageMimeTypes.Webp
    ]);

    public static string MimeTypeValidationError(IEnumerable<string> types)
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
