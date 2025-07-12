using FluentValidation;
using VictoryCenter.BLL.Commands.Images.Update;
using VictoryCenter.BLL.DTOs.Images;

// using VictoryCenter.BLL.Commands.Images.Update;

namespace VictoryCenter.BLL.Validators.Images;

public class UpdateImageValidator : AbstractValidator<UpdateImageCommand>
{
    public UpdateImageValidator()
    {
        RuleFor(x => x.updateImageDto).NotNull().WithMessage("UpdateImageDto cannot be null");
        RuleFor(x => x.updateImageDto).SetValidator(new BaseImageValidator<UpdateImageDTO>());
    }
}
