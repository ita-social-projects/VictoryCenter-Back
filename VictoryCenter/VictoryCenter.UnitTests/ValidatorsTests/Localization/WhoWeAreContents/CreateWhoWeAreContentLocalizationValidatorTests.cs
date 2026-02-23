using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.WhoWeAreContents.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.Validators.Localization.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.WhoWeAreContents;

public class CreateWhoWeAreContentLocalizationValidatorTests
{
    private readonly CreateWhoWeAreContentLocalizationValidator _validator;

    public CreateWhoWeAreContentLocalizationValidatorTests()
    {
        _validator = new CreateWhoWeAreContentLocalizationValidator();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenContentLocalizationDtosIsNull()
    {
        var command = new CreateWhoWeAreContentLocalizationCommand(SectionType.Main, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ContentLocalizationDtos)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateWhoWeAreContentLocalizationCommand.ContentLocalizationDtos)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenEntityIdIsNotPositive(long invalidEntityId)
    {
        var command = BuildCommand(SectionType.Main, entityId: invalidEntityId, languageId: 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("ContentLocalizationDtos[0].EntityId")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateWhoWeAreContentLocalizationDto.EntityId)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long invalidLanguageId)
    {
        var command = BuildCommand(SectionType.Main, entityId: 1, languageId: invalidLanguageId);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("ContentLocalizationDtos[0].LanguageId")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateWhoWeAreContentLocalizationDto.LanguageId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var shortTitle = new string('A', WhoWeAreContentLocalizationConstants.ValidationTitleRules.MinLen - 1);
        var minDescLen = WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[SectionType.Main].MinLen;
        var command = BuildCommand(
            SectionType.Main,
            title: shortTitle,
            description: new string('A', minDescLen));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("ContentLocalizationDtos[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentLocalizationDto.Title),
                WhoWeAreContentLocalizationConstants.ValidationTitleRules.MinLen));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var longTitle = new string('A', WhoWeAreContentLocalizationConstants.ValidationTitleRules.MaxLen + 1);
        var minDescLen = WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[SectionType.Main].MinLen;
        var command = BuildCommand(
            SectionType.Main,
            title: longTitle,
            description: new string('A', minDescLen));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("ContentLocalizationDtos[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentLocalizationDto.Title),
                WhoWeAreContentLocalizationConstants.ValidationTitleRules.MaxLen));
    }

    [Theory]
    [InlineData(SectionType.Main)]
    [InlineData(SectionType.WhatWeDo)]
    [InlineData(SectionType.People)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooShort(SectionType sectionType)
    {
        var minLen = WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[sectionType].MinLen;
        var shortDescription = new string('A', minLen - 1);
        var command = BuildCommand(sectionType, description: shortDescription);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("ContentLocalizationDtos[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentLocalizationDto.Description), minLen));
    }

    [Theory]
    [InlineData(SectionType.Main)]
    [InlineData(SectionType.WhatWeDo)]
    [InlineData(SectionType.People)]
    public void Validate_ShouldHaveError_WhenDescriptionIsTooLong(SectionType sectionType)
    {
        var maxLen = WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[sectionType].MaxLen;
        var longDescription = new string('A', maxLen + 1);
        var command = BuildCommand(sectionType, description: longDescription);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("ContentLocalizationDtos[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentLocalizationDto.Description), maxLen));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenCommandIsValid()
    {
        var minDescLen = WhoWeAreContentLocalizationConstants.ValidationDescriptionRules[SectionType.Main].MinLen;
        var minTitleLen = WhoWeAreContentLocalizationConstants.ValidationTitleRules.MinLen;

        var command = BuildCommand(
            SectionType.Main,
            title: new string('A', minTitleLen),
            description: new string('A', minDescLen));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateWhoWeAreContentLocalizationCommand BuildCommand(
        SectionType sectionType,
        long entityId = 1,
        long languageId = 1,
        string? title = null,
        string? description = null)
    {
        return new CreateWhoWeAreContentLocalizationCommand(
            sectionType,
            new List<CreateWhoWeAreContentLocalizationDto>
            {
                new()
                {
                    EntityId = entityId,
                    LanguageId = languageId,
                    Title = title,
                    Description = description
                }
            });
    }
}
