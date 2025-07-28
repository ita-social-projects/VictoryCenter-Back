using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Programs.Update;

namespace VictoryCenter.BLL.Validators.Programs;

public class UpdateProgramValidator : AbstractValidator<UpdateProgramCommand>
{
    public UpdateProgramValidator()
    {
        RuleFor(command => command.updateProgramDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"))
            .MaximumLength(ProgramConstants.MaxNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Name", ProgramConstants.MaxNameLength))
            .MinimumLength(ProgramConstants.MinNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramConstants.MinNameLength));

        RuleFor(command => command.updateProgramDto.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Description"))
            .MaximumLength(ProgramConstants.MaxDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Description", ProgramConstants.MaxDescriptionLength))
            .MinimumLength(ProgramConstants.MinDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Description", ProgramConstants.MinDescriptionLength));

        RuleFor(command => command.updateProgramDto.CategoriesId)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired("Categories-list"));
    }
}
