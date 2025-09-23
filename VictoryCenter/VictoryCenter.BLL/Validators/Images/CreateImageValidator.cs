using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Images.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Validators.Images;

public class CreateImageValidator : AbstractValidator<CreateImageCommand>
{
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private static readonly int MaxImageSize = 3 * 1024 * 1024;

    public CreateImageValidator()
    {
        RuleFor(x => x.CreateImageDto).NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageCommand.CreateImageDto)));
        RuleFor(x => x.CreateImageDto.Base64)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageDto.Base64)))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError)
            .Must(IsValidSize).WithMessage(ImageConstants.InvalidImageSize);

        RuleFor(x => x.CreateImageDto.MimeType)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageDto.MimeType)))
            .Must(mimeType => AllowedMimeTypes.Contains(mimeType))
            .WithMessage(ImageConstants.MimeTypeValidationError(AllowedMimeTypes));
    }

    private static bool IsValidSize(string base64)
    {
        try
        {
            var commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0)
            {
                base64 = base64[(commaIndex + 1)..];
            }

            int padding = base64.EndsWith("==") ? 2 : base64.EndsWith("=") ? 1 : 0;
            double originalSize = (base64.Length * 3 / 4.0) - padding;

            return originalSize <= MaxImageSize;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        Span<byte> buffer = new(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
