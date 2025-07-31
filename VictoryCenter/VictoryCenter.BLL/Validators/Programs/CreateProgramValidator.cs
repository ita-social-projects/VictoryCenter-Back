using FluentValidation;
using VictoryCenter.BLL.Commands.Programs.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Programs;

public class CreateProgramValidator : AbstractValidator<CreateProgramCommand>
{
    public CreateProgramValidator()
    {
        RuleFor(command => command.createProgramDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"))
            .MaximumLength(ProgramConstants.MaxNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Name", ProgramConstants.MaxNameLength))
            .MinimumLength(ProgramConstants.MinNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramConstants.MinNameLength));

        RuleFor(command => command.createProgramDto.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Description"))
            .MaximumLength(ProgramConstants.MaxDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Description", ProgramConstants.MaxDescriptionLength))
            .MinimumLength(ProgramConstants.MinDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Description", ProgramConstants.MinDescriptionLength));

        RuleFor(command => command.createProgramDto.CategoriesId)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired("Categories-list"));
    }
}
