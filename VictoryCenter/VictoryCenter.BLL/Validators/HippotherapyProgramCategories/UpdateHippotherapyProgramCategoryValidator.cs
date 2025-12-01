using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Validators.HippotherapyProgramCategories;

public class UpdateHippotherapyProgramCategoryValidator : AbstractValidator<UpdateHippotherapyProgramCategoryCommand>
{
    public UpdateHippotherapyProgramCategoryValidator()
    {
        RuleFor(command => command.UpdateProgramCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(UpdateHippotherapyProgramCategoryDto.Name)))
            .MaximumLength(HippotherapyProgramCategoryConstants.MaxNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramCategoryDto.Name),
                HippotherapyProgramCategoryConstants.MaxNameLength))
            .MinimumLength(HippotherapyProgramCategoryConstants.MinNameLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateHippotherapyProgramCategoryDto.Name),
                HippotherapyProgramCategoryConstants.MinNameLength));
    }
}
