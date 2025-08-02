using FluentValidation;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.Programs;

public class BaseProgramValidator : AbstractValidator<CreateProgramDto>
{
    public BaseProgramValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"))
            .MaximumLength(ProgramConstants.MaxNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters("Name", ProgramConstants.MaxNameLength))
            .MinimumLength(ProgramConstants.MinNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters("Name", ProgramConstants.MinNameLength));

        RuleFor(x => x.Description)
            .MaximumLength(ProgramConstants.MaxDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters("Description", ProgramConstants.MaxDescriptionLength))
            .MinimumLength(ProgramConstants.MinDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters("Description", ProgramConstants.MinDescriptionLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Description"))
            .When(x => x.Status == Status.Published);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ProgramConstants.UnknownStatus);

        RuleFor(x => x.CategoriesId)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired("Categories-list"));
    }
}
