using FluentValidation;

namespace VictoryCenter.BLL.Validators.Images;

public abstract class BaseImageValidator<TImageCommand> : AbstractValidator<TImageCommand>
{
    protected const int MaxImageSizeInMb = 3;
    protected const int BytesPerMb = 1024 * 1024;
    protected const int MaxImageSizeInBytes = MaxImageSizeInMb * BytesPerMb;

    protected static readonly string[] AllowedMimeTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    ];

    private const char Base64PaddingChar = '=';
    private const int DoublePaddingLength = 2;
    private const int SinglePaddingLength = 1;

    private const int ZeroPaddingLength = 0;
    private const int Base64BytesPer3Chars = 3;
    private const double Base64CharsPer3Bytes = 4.0;

    private const char CommaChar = ',';

    protected static bool IsValidSize(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        var commaIndex = base64.IndexOf(CommaChar);
        if (commaIndex >= 0)
        {
            base64 = base64[(commaIndex + 1)..];
        }

        int padding = base64 switch
        {
            not null when base64.EndsWith(new string(Base64PaddingChar, DoublePaddingLength), StringComparison.InvariantCulture)
                => DoublePaddingLength,
            not null when base64.EndsWith(Base64PaddingChar)
                => SinglePaddingLength,
            _ => ZeroPaddingLength
        };

        double originalSize = (base64!.Length * Base64BytesPer3Chars / Base64CharsPer3Bytes) - padding;

        return originalSize <= MaxImageSizeInBytes;
    }

    protected static bool IsValidBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        Span<byte> buffer = new(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
