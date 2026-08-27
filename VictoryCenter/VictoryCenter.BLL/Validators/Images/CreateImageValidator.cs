using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Images.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Images;
using VictoryCenter.BLL.Interfaces.ImageValidation;

namespace VictoryCenter.BLL.Validators.Images;

public class CreateImageValidator : BaseImageValidator<CreateImageCommand>
{
    public CreateImageValidator(IImageContentValidator imageContentValidator)
    {
        RuleFor(x => x.CreateImageDto).NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageCommand.CreateImageDto)));
        When(x => x.CreateImageDto is not null, () =>
        {
            RuleFor(x => x.CreateImageDto.Base64)
                .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageDto.Base64)));

            RuleFor(x => x.CreateImageDto.MimeType)
                .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateImageDto.MimeType)));

            AddImageContentRule(
                command => command.CreateImageDto.Base64,
                command => command.CreateImageDto.MimeType,
                imageContentValidator);
        });
    }
}
