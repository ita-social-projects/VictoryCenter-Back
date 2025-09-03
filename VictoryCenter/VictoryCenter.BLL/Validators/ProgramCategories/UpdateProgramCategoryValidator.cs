using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ProgramCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;

namespace VictoryCenter.BLL.Validators.ProgramCategories;

public class UpdateProgramCategoryValidator : AbstractValidator<UpdateProgramCategoryCommand>
{
    public UpdateProgramCategoryValidator()
    {
        RuleFor(command => command.updateProgramCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateProgramCategoryDto.Name)))
            .MaximumLength(ProgramCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UpdateProgramCategoryDto.Name), ProgramCategoryConstants.MaxNameLength))
            .MinimumLength(ProgramCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UpdateProgramCategoryDto.Name), ProgramCategoryConstants.MinNameLength));
    }
}
