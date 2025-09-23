using FluentValidation;

namespace VictoryCenter.BLL.Validators.Images;

public abstract class BaseImageValidator<TImageCommand> : AbstractValidator<TImageCommand>
{
    protected static readonly int MaxImageSize = 3 * 1024 * 1024;
    protected static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];

    protected static bool IsValidSize(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        var commaIndex = base64.IndexOf(',');
        if (commaIndex >= 0)
        {
            base64 = base64[(commaIndex + 1)..];
        }

        int padding = base64 switch
        {
            not null when base64.EndsWith("==", StringComparison.InvariantCulture) => 2,
            not null when base64.EndsWith('=') => 1,
            _ => 0
        };
        double originalSize = (base64!.Length * 3 / 4.0) - padding;

        return originalSize <= MaxImageSize;
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
