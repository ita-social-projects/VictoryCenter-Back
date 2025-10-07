using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.WhoWeAreSections;

internal class WhoWeAreSectionValidator : AbstractValidator<CreateWhoWeAreContentDto>
{
    public WhoWeAreSectionValidator(SectionType sectionType)
    {
        if (sectionType == SectionType.Main)
        {
            RuleFor(x => x.Title)
                .MinimumLength(WhoWeAreConstants.ValidationTitleRules.MinLen)
                .WithMessage(
                    ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                        nameof(CreateWhoWeAreContentDto.Title), WhoWeAreConstants.ValidationTitleRules.MinLen))
                .MaximumLength(WhoWeAreConstants.ValidationTitleRules.MaxLen)
                .WithMessage(
                    ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                        nameof(CreateWhoWeAreContentDto.Title), WhoWeAreConstants.ValidationTitleRules.MaxLen));
        }

        RuleFor(x => x.Description)
            .MinimumLength(WhoWeAreConstants.ValidationDescriptionRules[sectionType].MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentDto.Description), WhoWeAreConstants.ValidationDescriptionRules[sectionType].MinLen))
            .MaximumLength(WhoWeAreConstants.ValidationDescriptionRules[sectionType].MaxLen)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CreateWhoWeAreContentDto.Description), WhoWeAreConstants.ValidationDescriptionRules[sectionType].MaxLen));
    }
}
