using FluentValidation;
using VictoryCenter.BLL.Commands.Images.Update;

// using VictoryCenter.BLL.Commands.Images.Update;

namespace VictoryCenter.BLL.Validators.Images;

public class UpdateImageValidator : AbstractValidator<UpdateImageCommand>
{
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    public UpdateImageValidator()
    {
        RuleFor(x => x.updateImageDto).NotEmpty().WithMessage("CreateImageDto cannot be null");
        RuleFor(x => x.updateImageDto.Base64)
            .NotEmpty().WithMessage("Base64 content is required")
            .Must(IsValidBase64).WithMessage("Base64 content is invalid");

        RuleFor(x => x.updateImageDto.MimeType)
            .NotEmpty().WithMessage("MimeType is required")
            .Must(mimeType => AllowedMimeTypes.Contains(mimeType))
            .WithMessage($"MimeType must be one of the following: {string.Join(", ", AllowedMimeTypes)}");
    }

    private bool IsValidBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
