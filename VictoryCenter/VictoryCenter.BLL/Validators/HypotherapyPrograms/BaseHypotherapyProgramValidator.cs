using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyPrograms;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.HypotherapyPrograms;

public class BaseHypotherapyProgramValidator : AbstractValidator<CreateHypotherapyProgramDto>
{
    public BaseHypotherapyProgramValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Name)))
            .MaximumLength(HypotherapyProgramConstants.MaxNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HypotherapyProgramDto.Name), HypotherapyProgramConstants.MaxNameLength))
            .MinimumLength(HypotherapyProgramConstants.MinNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HypotherapyProgramDto.Name), HypotherapyProgramConstants.MinNameLength));

        RuleFor(x => x.Description)
            .MaximumLength(HypotherapyProgramConstants.MaxDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HypotherapyProgramDto.Description), HypotherapyProgramConstants.MaxDescriptionLength))
            .MinimumLength(HypotherapyProgramConstants.MinDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HypotherapyProgramDto.Description), HypotherapyProgramConstants.MinDescriptionLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Description)))
            .When(x => x.Status == Status.Published);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ErrorMessagesConstants.UnknownStatusValue);

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HypotherapyProgramDto.Categories)));
    }
}
