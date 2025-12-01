using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Validators.HippotherapyProgramCategories;

public class CreateHippotherapyProgramCategoryValidator : AbstractValidator<CreateHippotherapyProgramCategoryCommand>
{
    public CreateHippotherapyProgramCategoryValidator()
    {
        RuleFor(command => command.CreateProgramCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(HippotherapyProgramCategoryDto.Name)))
            .MaximumLength(HippotherapyProgramCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(HippotherapyProgramCategoryDto.Name),
                HippotherapyProgramCategoryConstants.MaxNameLength))
            .MinimumLength(HippotherapyProgramCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(HippotherapyProgramCategoryDto.Name),
                HippotherapyProgramCategoryConstants.MinNameLength));
    }
}
