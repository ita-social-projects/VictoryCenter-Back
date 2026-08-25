using System.Buffers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Memory;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.Interfaces.ImageValidation;

namespace VictoryCenter.BLL.Services.ImageValidation;

/// <summary>
/// Validates Base64-encoded raster image content before it is persisted.
/// </summary>
public sealed class ImageContentValidator : IImageContentValidator
{
    private const int BitsPerByte = 8;
    private const uint MaxFramesToIdentify = 2;
    private const uint MaxAllowedFrames = 1;

    private static readonly Configuration DecoderConfiguration = CreateDecoderConfiguration();

    private static readonly DecoderOptions IdentifyOptions = new()
    {
        Configuration = DecoderConfiguration,
        MaxFrames = MaxFramesToIdentify,
        SkipMetadata = true
    };

    /// <summary>
    /// Validates the encoded size, Base64 representation, actual image format, dimensions,
    /// decoded memory estimate, MIME type, and frame count.
    /// </summary>
    /// <param name="base64">Raw Base64-encoded image bytes without a data-URL prefix.</param>
    /// <param name="mimeType">MIME type declared by the client.</param>
    /// <returns>A validation result containing the first detected failure, or success.</returns>
    public ImageContentValidationResult Validate(string base64, string mimeType)
    {
        if (base64.Length > ImageConstants.MaxBase64Length)
        {
            return Failure(nameof(CreateImageDto.Base64), ImageConstants.InvalidImageSize);
        }

        if (!ImageConstants.AllowedMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
        {
            return Failure(
                nameof(CreateImageDto.MimeType),
                ImageConstants.MimeTypeValidationError(ImageConstants.AllowedMimeTypes));
        }

        byte[] buffer = ArrayPool<byte>.Shared.Rent(base64.Length);
        try
        {
            if (!Convert.TryFromBase64String(base64, buffer, out var decodedLength))
            {
                return Failure(nameof(CreateImageDto.Base64), ImageConstants.Base64ValidationError);
            }

            if (decodedLength > ImageConstants.MaxImageSizeInBytes)
            {
                return Failure(nameof(CreateImageDto.Base64), ImageConstants.InvalidImageSize);
            }

            using var stream = new MemoryStream(buffer, 0, decodedLength, false, true);
            return ValidateDecodedContent(stream, mimeType);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer, clearArray: true);
        }
    }

    /// <summary>
    /// Identifies and decodes the image using only the explicitly supported decoders.
    /// </summary>
    /// <param name="stream">Decoded image bytes positioned at the beginning.</param>
    /// <param name="mimeType">MIME type declared by the client.</param>
    /// <returns>A validation result for the decoded image content.</returns>
    private static ImageContentValidationResult ValidateDecodedContent(Stream stream, string mimeType)
    {
        try
        {
            ImageInfo imageInfo = Image.Identify(IdentifyOptions, stream);
            IImageFormat? imageFormat = imageInfo.Metadata.DecodedImageFormat;

            if (imageFormat is null || !IsAllowedFormat(imageFormat))
            {
                return Failure(nameof(CreateImageDto.Base64), ImageConstants.UnsupportedImageContentFormat);
            }

            if (!DoesMimeTypeMatch(imageFormat, mimeType))
            {
                return Failure(nameof(CreateImageDto.MimeType), ImageConstants.ImageMimeTypeMismatch);
            }

            if (imageInfo.Width > ImageConstants.MaxImageWidth || imageInfo.Height > ImageConstants.MaxImageHeight)
            {
                return Failure(nameof(CreateImageDto.Base64), ImageConstants.ImageDimensionsExceeded);
            }

            long pixelCount = (long)imageInfo.Width * imageInfo.Height;
            if (pixelCount > ImageConstants.MaxImagePixelCount)
            {
                return Failure(nameof(CreateImageDto.Base64), ImageConstants.ImagePixelCountExceeded);
            }

            long decodedBits = pixelCount * imageInfo.PixelType.BitsPerPixel;
            long decodedImageSize = (decodedBits + BitsPerByte - 1) / BitsPerByte;
            if (decodedImageSize > ImageConstants.MaxDecodedImageSizeInBytes)
            {
                return Failure(nameof(CreateImageDto.Base64), ImageConstants.DecodedImageSizeExceeded);
            }

            stream.Position = 0;
            using Image image = Image.Load(IdentifyOptions, stream);

            if (image.Frames.Count > MaxAllowedFrames)
            {
                return Failure(nameof(CreateImageDto.Base64), ImageConstants.AnimatedImageNotSupported);
            }

            return ImageContentValidationResult.Success;
        }
        catch (UnknownImageFormatException)
        {
            return Failure(nameof(CreateImageDto.Base64), ImageConstants.InvalidImageContent);
        }
        catch (ImageFormatException)
        {
            return Failure(nameof(CreateImageDto.Base64), ImageConstants.InvalidImageContent);
        }
        catch (ArgumentException)
        {
            return Failure(nameof(CreateImageDto.Base64), ImageConstants.InvalidImageContent);
        }
        catch (InvalidMemoryOperationException)
        {
            return Failure(nameof(CreateImageDto.Base64), ImageConstants.DecodedImageSizeExceeded);
        }
        catch (NotSupportedException)
        {
            return Failure(nameof(CreateImageDto.Base64), ImageConstants.UnsupportedImageContentFormat);
        }
    }

    /// <summary>
    /// Determines whether the detected image format is supported by the upload API.
    /// </summary>
    /// <param name="format">Format detected from the image bytes.</param>
    /// <returns><see langword="true"/> for JPEG, PNG, or WebP; otherwise <see langword="false"/>.</returns>
    private static bool IsAllowedFormat(IImageFormat format)
    {
        return format.Name.Equals(JpegFormat.Instance.Name, StringComparison.OrdinalIgnoreCase)
               || format.Name.Equals(PngFormat.Instance.Name, StringComparison.OrdinalIgnoreCase)
               || format.Name.Equals(WebpFormat.Instance.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Compares the detected image format with the client-declared MIME type.
    /// </summary>
    /// <param name="format">Format detected from the image bytes.</param>
    /// <param name="mimeType">MIME type declared by the client.</param>
    /// <returns><see langword="true"/> when the MIME type represents the detected format.</returns>
    private static bool DoesMimeTypeMatch(IImageFormat format, string mimeType)
    {
        return format.Name.Equals(JpegFormat.Instance.Name, StringComparison.OrdinalIgnoreCase)
            ? mimeType.Equals(ImageMimeTypes.Jpeg, StringComparison.OrdinalIgnoreCase)
              || mimeType.Equals(ImageMimeTypes.Jpg, StringComparison.OrdinalIgnoreCase)
            : mimeType.Equals(format.DefaultMimeType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a failed validation result for a DTO property.
    /// </summary>
    /// <param name="propertyName">Name of the DTO property associated with the failure.</param>
    /// <param name="errorMessage">Public validation error message.</param>
    /// <returns>A failed image-content validation result.</returns>
    private static ImageContentValidationResult Failure(string propertyName, string errorMessage)
    {
        return ImageContentValidationResult.Failure(propertyName, errorMessage);
    }

    /// <summary>
    /// Creates a dedicated decoder configuration with an explicit format allowlist and bounded allocations.
    /// </summary>
    /// <returns>The decoder configuration used for untrusted uploads.</returns>
    private static Configuration CreateDecoderConfiguration()
    {
        var configuration = new Configuration(
            new JpegConfigurationModule(),
            new PngConfigurationModule(),
            new WebpConfigurationModule())
        {
            MemoryAllocator = MemoryAllocator.Create(new MemoryAllocatorOptions
            {
                AllocationLimitMegabytes = ImageConstants.ImageDecoderAllocationLimitInMb,
                MaximumPoolSizeMegabytes = ImageConstants.ImageDecoderPoolSizeInMb
            })
        };

        return configuration;
    }
}
