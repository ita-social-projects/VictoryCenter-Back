using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Images.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;

namespace VictoryCenter.BLL.Validators.Images;

public class CreateImageValidator : BaseImageValidator<CreateImageCommand>
{
    public CreateImageValidator()
    {
        RuleFor(x => x.CreateImageDto).NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageCommand.CreateImageDto)));
        RuleFor(x => x.CreateImageDto.Base64)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageDto.Base64)))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError)
            .Must(IsValidSize).WithMessage(ImageConstants.InvalidImageSize);

        RuleFor(x => x.CreateImageDto.MimeType)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageDto.MimeType)))
            .Must(mimeType => AllowedMimeTypes.Contains(mimeType, StringComparer.InvariantCultureIgnoreCase))
            .WithMessage(ImageConstants.MimeTypeValidationError(AllowedMimeTypes));
    }
}
