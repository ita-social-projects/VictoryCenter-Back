using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;

namespace VictoryCenter.BLL.Validators.HypotherapyProgramCategories;

public class CreateHypotherapyProgramCategoryValidator : AbstractValidator<CreateHypotherapyProgramCategoryCommand>
{
    public CreateHypotherapyProgramCategoryValidator()
    {
        RuleFor(command => command.ProgramCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramCategoryDto.Name)))
            .MaximumLength(HypotherapyProgramCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HypotherapyProgramCategoryDto.Name), HypotherapyProgramCategoryConstants.MaxNameLength))
            .MinimumLength(HypotherapyProgramCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HypotherapyProgramCategoryDto.Name), HypotherapyProgramCategoryConstants.MinNameLength));
    }
}
