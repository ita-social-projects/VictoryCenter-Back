using FluentValidation;
using VictoryCenter.BLL.Commands.Images.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Images;

namespace VictoryCenter.BLL.Validators.Images;

public class CreateImageValidator : AbstractValidator<CreateImageCommand>
{
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    public CreateImageValidator()
    {
        RuleFor(x => x.CreateImageDto).NotEmpty().WithMessage(ImageConstants.CreateImageDtoCantBeNull);
        RuleFor(x => x.CreateImageDto.Base64)
            .NotEmpty().WithMessage(ImageConstants.FieldIsRequired(nameof(CreateImageDTO.Base64)))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError);

        RuleFor(x => x.CreateImageDto.MimeType)
            .NotEmpty().WithMessage(ImageConstants.FieldIsRequired(nameof(CreateImageDTO.MimeType)))
            .Must(mimeType => AllowedMimeTypes.Contains(mimeType))
            .WithMessage(ImageConstants.MimeTypeValidationError(AllowedMimeTypes));
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
