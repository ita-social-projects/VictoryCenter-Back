using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;

namespace VictoryCenter.BLL.Validators.Localization.History;

public class CreateHistorySectionContentLocalizationValidator : AbstractValidator<CreateHistorySectionContentLocalizationDto>
{
    public CreateHistorySectionContentLocalizationValidator()
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
    }
}
