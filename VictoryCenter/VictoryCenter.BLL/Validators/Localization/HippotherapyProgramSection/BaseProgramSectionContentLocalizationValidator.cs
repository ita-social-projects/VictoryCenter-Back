using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramSection;

public class BaseProgramSectionContentLocalizationValidator : AbstractValidator<BaseHippotherapyProgramSectionContentLocalizationDto>
{
    public BaseProgramSectionContentLocalizationValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(ProgramSectionContentLocalizationConstants.TitleMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Title),
                ProgramSectionContentLocalizationConstants.TitleMinLength))
            .MaximumLength(ProgramSectionContentLocalizationConstants.TitleMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Title),
                ProgramSectionContentLocalizationConstants.TitleMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Description)
            .MinimumLength(ProgramSectionContentLocalizationConstants.DescriptionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Description),
                ProgramSectionContentLocalizationConstants.DescriptionMinLength))
            .MaximumLength(ProgramSectionContentLocalizationConstants.DescriptionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Description),
                ProgramSectionContentLocalizationConstants.DescriptionMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Author)
            .MinimumLength(ProgramSectionContentLocalizationConstants.AuthorMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Author),
                ProgramSectionContentLocalizationConstants.AuthorMinLength))
            .MaximumLength(ProgramSectionContentLocalizationConstants.AuthorMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Author),
                ProgramSectionContentLocalizationConstants.AuthorMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Author));

        RuleFor(x => x.Question)
            .MinimumLength(ProgramSectionContentLocalizationConstants.QuestionMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Question),
                ProgramSectionContentLocalizationConstants.QuestionMinLength))
            .MaximumLength(ProgramSectionContentLocalizationConstants.QuestionMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Question),
                ProgramSectionContentLocalizationConstants.QuestionMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Question));

        RuleFor(x => x.Answer)
            .MinimumLength(ProgramSectionContentLocalizationConstants.AnswerMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Answer),
                ProgramSectionContentLocalizationConstants.AnswerMinLength))
            .MaximumLength(ProgramSectionContentLocalizationConstants.AnswerMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(BaseHippotherapyProgramSectionContentLocalizationDto.Answer),
                ProgramSectionContentLocalizationConstants.AnswerMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Answer));
    }
}
