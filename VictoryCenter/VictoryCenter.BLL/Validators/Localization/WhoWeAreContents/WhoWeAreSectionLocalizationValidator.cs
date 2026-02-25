using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.Localization.WhoWeAreContents;

internal class WhoWeAreSectionLocalizationValidator : AbstractValidator<UpdateWhoWeAreContentLocalizationDto>
{
    public WhoWeAreSectionLocalizationValidator(SectionType sectionType)
    {
        if (sectionType == SectionType.Main)
        {
            RuleFor(x => x.Title)
                .MinimumLength(WhoWeAreContentLocalizationConstants.ValidationTitleRules.MinLen)
                .WithMessage(
                    ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                        nameof(UpdateWhoWeAreContentLocalizationDto.Title), WhoWeAreContentLocalizationConstants.ValidationTitleRules.MinLen))
                .MaximumLength(WhoWeAreContentLocalizationConstants.ValidationTitleRules.MaxLen)
                .WithMessage(
                    ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                        nameof(UpdateWhoWeAreContentLocalizationDto.Title), WhoWeAreContentLocalizationConstants.ValidationTitleRules.MaxLen));
        }

        if (!WhoWeAreContentLocalizationConstants.ValidationDescriptionRules.ContainsKey(sectionType))
        {
            throw new ArgumentException(WhoWeAreContentLocalizationConstants.NoValidationRulesDefinedForSectionType(sectionType));
        }

        RuleFor(x => x.Description)
            .MinimumLength(WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[sectionType].MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateWhoWeAreContentLocalizationDto.Description), WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[sectionType].MinLen))
            .MaximumLength(WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[sectionType].MaxLen)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UpdateWhoWeAreContentLocalizationDto.Description), WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[sectionType].MaxLen));
    }
}
