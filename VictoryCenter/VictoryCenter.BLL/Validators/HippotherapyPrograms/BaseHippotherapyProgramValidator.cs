using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.HippotherapyPrograms;

public class BaseHippotherapyProgramValidator : AbstractValidator<CreateHippotherapyProgramDto>
{
    public BaseHippotherapyProgramValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Name)))
            .MaximumLength(HippotherapyProgramConstants.MaxNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.Name), HippotherapyProgramConstants.MaxNameLength))
            .MinimumLength(HippotherapyProgramConstants.MinNameLength)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.Name), HippotherapyProgramConstants.MinNameLength));

        RuleFor(x => x.Description)
            .MaximumLength(HippotherapyProgramConstants.MaxDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(HippotherapyProgramDto.Description), HippotherapyProgramConstants.MaxDescriptionLength))
            .MinimumLength(HippotherapyProgramConstants.MinDescriptionLength)
            .WithMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(HippotherapyProgramDto.Description), HippotherapyProgramConstants.MinDescriptionLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Description)))
            .When(x => x.Status == Status.Published);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ErrorMessagesConstants.UnknownStatusValue);

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(HippotherapyProgramDto.Categories)));
    }
}
