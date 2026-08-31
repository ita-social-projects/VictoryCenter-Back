using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Images.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.Interfaces.ImageValidation;

namespace VictoryCenter.BLL.Validators.Images;

public class UpdateImageValidator : BaseImageValidator<UpdateImageCommand>
{
    public UpdateImageValidator(IImageContentValidator imageContentValidator)
    {
        RuleFor(x => x.UpdateImageDto).NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageCommand.UpdateImageDto)));
        When(x => x.UpdateImageDto is not null, () =>
        {
            RuleFor(x => x.UpdateImageDto.Base64)
                .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.Base64)));

            RuleFor(x => x.UpdateImageDto.MimeType)
                .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateImageDto.MimeType)));

            AddImageContentRule(
                command => command.UpdateImageDto.Base64,
                command => command.UpdateImageDto.MimeType,
                imageContentValidator);
        });
    }
}
