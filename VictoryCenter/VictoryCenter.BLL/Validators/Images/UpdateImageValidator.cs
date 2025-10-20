using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Images.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Validators.Images;

public class UpdateImageValidator : BaseImageValidator<UpdateImageCommand>
{
    public UpdateImageValidator()
    {
        RuleFor(x => x.UpdateImageDto).NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageCommand.UpdateImageDto)));
        RuleFor(x => x.UpdateImageDto.Base64)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.Base64)))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError)
            .Must(IsValidSize).WithMessage(ImageConstants.InvalidImageSize);

        RuleFor(x => x.UpdateImageDto.MimeType)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.MimeType)))
            .Must(mimeType => AllowedMimeTypes.Contains(mimeType, StringComparer.InvariantCultureIgnoreCase))
            .WithMessage(ImageConstants.MimeTypeValidationError(AllowedMimeTypes));
    }
}
