using FluentValidation;
using VictoryCenter.BLL.Commands.Images.Create;
using VictoryCenter.BLL.DTOs.Images;

namespace VictoryCenter.BLL.Validators.Images;

public class CreateImageValidator : AbstractValidator<CreateImageCommand>
{
    public CreateImageValidator()
    {
        RuleFor(x => x.CreateImageDto).NotEmpty().WithMessage("CreateImageDto cannot be null");
        RuleFor(x => x.CreateImageDto).SetValidator(new BaseImageValidator<CreateImageDTO>());
    }
}
