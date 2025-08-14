using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Images.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Validators.Images;

public class UpdateImageValidator : AbstractValidator<UpdateImageCommand>
{
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    public UpdateImageValidator()
    {
        RuleFor(x => x.UpdateImageDto).NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageCommand.UpdateImageDto)));
        RuleFor(x => x.UpdateImageDto.Base64)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.Base64)))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError);

        RuleFor(x => x.UpdateImageDto.MimeType)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.MimeType)))
            .Must(mimeType => AllowedMimeTypes.Contains(mimeType))
            .WithMessage(ImageConstants.MimeTypeValidationError(AllowedMimeTypes));
    }

    private static bool IsValidBase64(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
