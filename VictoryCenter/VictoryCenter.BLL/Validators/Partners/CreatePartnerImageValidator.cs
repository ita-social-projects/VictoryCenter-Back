using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Validators.Partners;

public class CreatePartnerImageValidator : BaseImageValidator<CreatePartnerImageDto>
{
    public CreatePartnerImageValidator()
    {
        RuleFor(x => x.Base64)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.Base64)))
            .Must(IsValidBase64).WithMessage(ImageConstants.Base64ValidationError);

        RuleFor(x => x.MimeType)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.MimeType)))
            .Must(mimeType => PartnerConstants.AllowedImageMimeTypes.Contains(mimeType))
            .WithMessage(ImageConstants.MimeTypeValidationError(PartnerConstants.AllowedImageMimeTypes));
    }
}
