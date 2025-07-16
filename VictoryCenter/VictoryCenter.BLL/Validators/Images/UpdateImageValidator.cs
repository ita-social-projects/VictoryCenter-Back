using FluentValidation;
using VictoryCenter.BLL.Commands.Images.Update;
using VictoryCenter.BLL.Constants;

// using VictoryCenter.BLL.Commands.Images.Update;

namespace VictoryCenter.BLL.Validators.Images;

public class UpdateImageValidator : AbstractValidator<UpdateImageCommand>
{
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    public UpdateImageValidator()
    {
        RuleFor(x => x.updateImageDto).NotEmpty().WithMessage("CreateImageDto cannot be null");
        RuleFor(x => x.updateImageDto.Base64)
            .NotEmpty().WithMessage(ImageConstants.FieldIsRequired("Base64 content"))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError);

        RuleFor(x => x.updateImageDto.MimeType)
            .NotEmpty().WithMessage(ImageConstants.FieldIsRequired("MimeType field"))
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
