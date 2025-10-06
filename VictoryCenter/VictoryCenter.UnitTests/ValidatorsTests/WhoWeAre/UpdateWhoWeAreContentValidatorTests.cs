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
        var command = new UpdateWhoWeAreContentCommand((SectionType)999, new List<CreateWhoWeAreContentDto>());
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SectionType);
    }

    [Fact]
    public void ShouldHaveError_WhenContentItemIsNull()
    {
        var command =
            new UpdateWhoWeAreContentCommand(SectionType.Main, new List<CreateWhoWeAreContentDto?> { null } !);
        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Content[0]").WithErrorMessage("Content cannot be null.");
    }

    [Fact]
    public void ShouldHaveError_WhenTitleTooShort_ForMain()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<CreateWhoWeAreContentDto>
            {
                new() { Title = "short", Description = new string('A', 50), Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Content[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentDto.Title), 10));
    }

    [Fact]
    public void ShouldHaveError_WhenTitleTooLong_ForMain()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<CreateWhoWeAreContentDto>
            {
                new() { Title = new string('A', 51), Description = new string('A', 50), Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Content[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentDto.Title), 50));
    }

    [Fact]
    public void ShouldHaveError_WhenDescriptionTooShort()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.WhatWeDo,
            new List<CreateWhoWeAreContentDto>
            {
                new() { Title = "Valid Title", Description = "Short", Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Content[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentDto.Description), 10));
    }

    [Fact]
    public void ShouldHaveError_WhenDescriptionTooLong()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.WhatWeDo,
            new List<CreateWhoWeAreContentDto>
            {
                new() { Title = "Valid Title", Description = new string('A', 301), Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Content[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateWhoWeAreContentDto.Description), 300));
    }

    [Fact]
    public void ShouldNotHaveErrors_ForValidMainContent()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Main,
            new List<CreateWhoWeAreContentDto>
            {
                new() { Title = "Valid Title Here", Description = new string('A', 50), Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldNotHaveErrors_ForValidTeamContent()
    {
        var command = new UpdateWhoWeAreContentCommand(
            SectionType.Team,
            new List<CreateWhoWeAreContentDto>
            {
                new() { Title = null, Description = new string('A', 100), Id = 1 }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
