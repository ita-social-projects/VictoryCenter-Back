using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.WhoWeAreContent;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Validators.WhoWeAreSections;

internal class WhoWeAreSectionValidator : AbstractValidator<CreateWhoWeAreContentDto>
{
    private static readonly Dictionary<SectionType, (int MinLen, int MaxLen)> DescriptionRules = new()
    {
        { SectionType.Main, (10, 300) },
        { SectionType.WhatWeDo, (10, 300) },
        { SectionType.WhoWeSupport, (10, 300) },
        { SectionType.Team, (10, 360) },
        { SectionType.People, (10, 60) }
    };

    public WhoWeAreSectionValidator(SectionType sectionType)
    {
        if (sectionType == SectionType.Main)
        {
            RuleFor(x => x.Title)
                .MinimumLength(10)
                .WithMessage(
                    ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                        nameof(CreateWhoWeAreContentDto.Title), 10))
                .MaximumLength(50)
                .WithMessage(
                    ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                        nameof(CreateWhoWeAreContentDto.Title), 50));
        }

        RuleFor(x => x.Description)
            .MinimumLength(DescriptionRules[sectionType].MinLen)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentDto.Description), DescriptionRules[sectionType].MinLen))
            .MaximumLength(DescriptionRules[sectionType].MaxLen)
            .WithMessage(
                ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CreateWhoWeAreContentDto.Description), DescriptionRules[sectionType].MaxLen));
    }
}
