using FluentValidation;
using VictoryCenter.BLL.Commands.ProgramCategory.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.ProgramCategories;

public class CreateProgramCategoryValidator : AbstractValidator<CreateProgramCategoryCommand>
{
    public CreateProgramCategoryValidator()
    {
        RuleFor(command => command.programCategoryDto.Name)
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
