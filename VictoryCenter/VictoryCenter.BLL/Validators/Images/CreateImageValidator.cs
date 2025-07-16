using FluentValidation;
using VictoryCenter.BLL.Commands.Images.Create;

namespace VictoryCenter.BLL.Validators.Images;

public class CreateImageValidator : AbstractValidator<CreateImageCommand>
{
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    public CreateImageValidator()
    {
        RuleFor(x => x.CreateImageDto).NotEmpty().WithMessage("CreateImageDto cannot be null");
        RuleFor(x => x.CreateImageDto.Base64)
            .NotEmpty().WithMessage("Base64 content is required")
            .Must(IsValidBase64).WithMessage("Base64 content is invalid");

        RuleFor(x => x.CreateImageDto.MimeType)
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
