using FluentValidation;
using VictoryCenter.BLL.Commands.ProgramCategories.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.ProgramCategories;

public class UpdateProgramCategoryValidator : AbstractValidator<UpdateProgramCategoryCommand>
{
    public UpdateProgramCategoryValidator()
    {
        RuleFor(command => command.updateProgramCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"))
            .MaximumLength(ProgramCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Name",  ProgramCategoryConstants.MaxNameLength))
            .MinimumLength(ProgramCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramCategoryConstants.MinNameLength));
    }
}
