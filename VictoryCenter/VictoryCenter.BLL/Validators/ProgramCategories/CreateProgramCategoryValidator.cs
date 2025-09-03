using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ProgramCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;

namespace VictoryCenter.BLL.Validators.ProgramCategories;

public class CreateProgramCategoryValidator : AbstractValidator<CreateProgramCategoryCommand>
{
    public CreateProgramCategoryValidator()
    {
        RuleFor(command => command.programCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ProgramCategoryDto.Name)))
            .MaximumLength(ProgramCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(ProgramCategoryDto.Name), ProgramCategoryConstants.MaxNameLength))
            .MinimumLength(ProgramCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ProgramCategoryDto.Name), ProgramCategoryConstants.MinNameLength));
    }
}
