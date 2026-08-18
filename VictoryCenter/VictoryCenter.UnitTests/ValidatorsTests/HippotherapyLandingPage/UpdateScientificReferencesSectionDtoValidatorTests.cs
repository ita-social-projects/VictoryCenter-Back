using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyLandingPage.Sections;
using VictoryCenter.BLL.Validators.HippotherapyLandingPage.Dto;

namespace VictoryCenter.UnitTests.ValidatorsTests.HippotherapyLandingPage;

public class UpdateScientificReferencesSectionDtoValidatorTests
{
    private readonly UpdateScientificReferencesSectionDtoValidator _validator =
        new(new UpdateScientificReferenceDtoValidator());

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_TitleIsNullOrEmpty_ShouldHaveError(string? title)
    {
        // Arrange
        var dto = GetValidDto() with { Title = title! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferencesSectionDto.Title)));
    }

    [Fact]
    public void Validate_TitleIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('A', HippotherapyLandingPageConstants.TitleMinLength - 1);
        var dto = GetValidDto() with { Title = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateScientificReferencesSectionDto.Title), HippotherapyLandingPageConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_TitleIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.ScientificReferencesTitleMaxLength + 1);
        var dto = GetValidDto() with { Title = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateScientificReferencesSectionDto.Title), HippotherapyLandingPageConstants.ScientificReferencesTitleMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_DescriptionIsNullOrEmpty_ShouldHaveError(string? description)
    {
        // Arrange
        var dto = GetValidDto() with { Description = description! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferencesSectionDto.Description)));
    }

    [Fact]
    public void Validate_DescriptionIsTooShort_ShouldHaveError()
    {
        // Arrange
        var tooShort = new string('A', HippotherapyLandingPageConstants.TextMinLength - 1);
        var dto = GetValidDto() with { Description = tooShort };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumVisibleLengthOfNCharacters(
                  nameof(UpdateScientificReferencesSectionDto.Description), HippotherapyLandingPageConstants.TextMinLength));
    }

    [Fact]
    public void Validate_DescriptionIsTooLong_ShouldHaveError()
    {
        // Arrange
        var tooLong = new string('A', HippotherapyLandingPageConstants.ScientificReferencesDescriptionMaxLength + 1);
        var dto = GetValidDto() with { Description = tooLong };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumVisibleLengthOfNCharacters(
                  nameof(UpdateScientificReferencesSectionDto.Description), HippotherapyLandingPageConstants.ScientificReferencesDescriptionMaxLength));
    }

    [Fact]
    public void Validate_ScientificReferencesIsNull_ShouldHaveError()
    {
        // Arrange
        var dto = GetValidDto() with { ScientificReferences = null! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ScientificReferences)
              .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateScientificReferencesSectionDto.ScientificReferences)));
    }

    [Fact]
    public void Validate_ScientificReferencesIsEmpty_ShouldHaveError()
    {
        // Arrange
        var dto = GetValidDto() with { ScientificReferences = [] };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ScientificReferences)
              .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                  nameof(UpdateScientificReferencesSectionDto.ScientificReferences)));
    }

    [Fact]
    public void Validate_ScientificReferencesHasDuplicateIds_ShouldHaveError()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            ScientificReferences =
            [
                GetValidReference() with { Id = 1 },
                GetValidReference() with { Id = 1 },
            ],
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ScientificReferences)
              .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                  nameof(UpdateScientificReferencesSectionDto.ScientificReferences)));
    }

    [Fact]
    public void Validate_ScientificReferencesWithMultipleNullIds_ShouldNotHaveUniqueValuesError()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            ScientificReferences =
            [
                GetValidReference() with { Id = null },
                GetValidReference() with { Id = null },
            ],
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        Assert.DoesNotContain(
            result.Errors,
            e => e.ErrorMessage == ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(UpdateScientificReferencesSectionDto.ScientificReferences)));
    }

    [Fact]
    public void Validate_ReferenceItemIsInvalid_ShouldPropagateNestedError()
    {
        // Arrange
        var dto = GetValidDto() with { ScientificReferences = [GetValidReference() with { Name = string.Empty }] };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor("ScientificReferences[0].Name");
    }

    [Fact]
    public void Validate_ValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = GetValidDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateScientificReferenceDto GetValidReference() => new()
    {
        Id = null,
        Name = new string('A', HippotherapyLandingPageConstants.ScientificReferenceNameMinLength),
        Url = "https://example.com/reference",
    };

    private static UpdateScientificReferencesSectionDto GetValidDto() => new()
    {
        Title = new string('A', HippotherapyLandingPageConstants.TitleMinLength),
        Description = new string('A', HippotherapyLandingPageConstants.TextMinLength),
        ScientificReferences = [GetValidReference()],
    };
}
