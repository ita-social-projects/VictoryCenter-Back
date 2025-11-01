using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.WhoWeAre.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.Validators.WhoWeAreSections;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.WhoWeAre;

public class UpdateWhoWeAreContentValidatorTests
{
    private readonly UpdateWhoWeAreContentValidator _validator;

    public UpdateWhoWeAreContentValidatorTests()
    {
        _validator = new UpdateWhoWeAreContentValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenSectionTypeIsInvalid()
    {
        var command = new UpdateWhoWeAreContentCommand((SectionType)999, new List<UpdateWhoWeAreContentDto>());
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SectionType);
    }

    [Fact]
    public void ShouldHaveError_WhenContentItemIsNull()
    {
        var command =
            new UpdateWhoWeAreContentCommand(SectionType.Main, new List<UpdateWhoWeAreContentDto> { null! } );
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Contents[0]")
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainNullElements(nameof(UpdateWhoWeAreContentCommand.Contents)));
    }

    [Fact]
    public void ShouldHaveError_WhenTitleTooShort_ForMain()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { Title = "short", ContentType = ContentType.Title, Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Contents[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateWhoWeAreContentDto.Title), 10));
    }

    [Fact]
    public void ShouldHaveError_WhenTitleTooLong_ForMain()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { Title = new string('A', 51), ContentType = ContentType.Title, Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Contents[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateWhoWeAreContentDto.Title), 50));
    }

    [Theory]
    [InlineData(SectionType.Main)]
    [InlineData(SectionType.WhatWeDo)]
    [InlineData(SectionType.WhoWeSupport)]
    [InlineData(SectionType.Team)]
    [InlineData(SectionType.People)]
    public void ShouldHaveError_WhenDescriptionTooShort(SectionType sectionType)
    {
        var command = new UpdateWhoWeAreContentCommand(
            sectionType,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { ContentType = ContentType.Description, Description = "Short", Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Contents[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateWhoWeAreContentDto.Description),  WhoWeAreConstants.ValidationDescriptionRules[sectionType].MinLen));
    }

    [Theory]
    [InlineData(SectionType.Main)]
    [InlineData(SectionType.WhatWeDo)]
    [InlineData(SectionType.WhoWeSupport)]
    [InlineData(SectionType.Team)]
    [InlineData(SectionType.People)]
    public void ShouldHaveError_WhenDescriptionTooLong(SectionType sectionType)
    {
        var command = new UpdateWhoWeAreContentCommand(
            sectionType,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { ContentType = ContentType.Description, Description = new string('A', 401), Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Contents[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateWhoWeAreContentDto.Description), WhoWeAreConstants.ValidationDescriptionRules[sectionType].MaxLen));
    }

    [Fact]
    public void ShouldNotHaveErrors_ForValidMainContent()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { ContentType = ContentType.Title, Title = "Valid Title", Id = 1 },
                new() { ContentType = ContentType.Description, Description = "Valid description", Id = 2 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveErrors_ForValidTeamContent()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Team,
            new List<UpdateWhoWeAreContentDto>
            {
                new() { ContentType = ContentType.Description, Description = new string('A', 100), Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
