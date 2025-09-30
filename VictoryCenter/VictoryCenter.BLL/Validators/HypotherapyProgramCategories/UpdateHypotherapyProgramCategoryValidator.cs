using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;

namespace VictoryCenter.BLL.Validators.HypotherapyProgramCategories;

public class UpdateHypotherapyProgramCategoryValidator : AbstractValidator<UpdateHypotherapyProgramCategoryCommand>
{
    public UpdateHypotherapyProgramCategoryValidator()
    {
        RuleFor(command => command.UpdateProgramCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateHypotherapyProgramCategoryDto.Name)))
            .MaximumLength(HypotherapyProgramCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateHypotherapyProgramCategoryDto.Name), HypotherapyProgramCategoryConstants.MaxNameLength))
            .MinimumLength(HypotherapyProgramCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateHypotherapyProgramCategoryDto.Name), HypotherapyProgramCategoryConstants.MinNameLength));
    }
}
