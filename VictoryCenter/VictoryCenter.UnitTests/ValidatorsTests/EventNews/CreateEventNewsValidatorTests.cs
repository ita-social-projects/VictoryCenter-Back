using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Validators.EventNews;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class CreateEventNewsValidatorTests
{
    private readonly CreateEventNewsValidator _validator = new(new BaseEventNewsValidator());

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDraftIsEmpty()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            CategoryId = 1
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDraftLocalizationsAreNull()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            CategoryId = 1,
            Localizations = null!
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveErrors_WhenPublishedCollectionsAreNull()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Published,
            PublishedAt = DateTimeOffset.UtcNow,
            PreviewImageId = 1,
            Localizations = null!
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.CategoryId);
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Localizations);
    }

    [Fact]
    public void Validate_ShouldHaveErrors_WhenPublishedRequiredFieldsAreMissing()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Published
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.PublishedAt);
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.PreviewImageId);
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.CategoryId);
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Localizations);
    }

    [Fact]
    public void Validate_ShouldHaveErrors_WhenPublishedLocalizationTextIsMissing()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Published,
            PublishedAt = DateTimeOffset.UtcNow,
            PreviewImageId = 1,
            CategoryId = 1,
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1
                },
            ]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateEventNewsDto.Localizations[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsLocalizationDto.Title)));
        result.ShouldHaveValidationErrorFor("CreateEventNewsDto.Localizations[0].Description")
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsLocalizationDto.Description)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = new string('a', EventNewsConstants.TitleMaxLength + 1)
                },
            ]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateEventNewsDto.Localizations[0].Title")
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateEventNewsLocalizationDto.Title),
                EventNewsConstants.TitleMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCategoryIdIsNotPositive()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            CategoryId = 0
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.CategoryId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateEventNewsDto.CategoryId)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedTitleIsMissing()
    {
        // Arrange
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Title = null,
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedDescriptionIsMissing()
    {
        // Arrange
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Description = null,
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateEventNewsDto.Description)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Title = new string('a', EventNewsConstants.TitleMaxLength + 1),
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateEventNewsDto.Title),
                EventNewsConstants.TitleMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Description = new string('a', EventNewsConstants.DescriptionMaxLength + 1),
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateEventNewsDto.Description),
                EventNewsConstants.DescriptionMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedTitleMinimumLengthIsNotMet()
    {
        // Arrange
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Title = new string('a', EventNewsConstants.TitleMinLength - 1),
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateEventNewsDto.Title),
                EventNewsConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedDescriptionMinimumLengthIsNotMet()
    {
        // Arrange
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Description = new string('a', EventNewsConstants.DescriptionMinLength - 1),
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateEventNewsDto.Description),
                EventNewsConstants.DescriptionMinLength));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(EventNewsConstants.AdditionalDescriptionMaxLength + 1)]
    public void Validate_ShouldHaveError_WhenAdditionalDescriptionLengthIsOutOfRange(int length)
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            AdditionalDescription = new string('a', length),
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.AdditionalDescription);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEnglishResourceIsTooLong()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            ResourceEn = new string('a', EventNewsConstants.ResourceMaxLength + 1),
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.ResourceEn)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateEventNewsDto.ResourceEn),
                EventNewsConstants.ResourceMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenLocalizationAdditionalDescriptionIsTooLong()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    Title = "Event News Title",
                    AdditionalDescription = new string('a', EventNewsConstants.AdditionalDescriptionMaxLength + 1),
                },
            ]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateEventNewsDto.Localizations[0].AdditionalDescription");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenLocalizationHasOnlyAdditionalDescription()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            Localizations =
            [
                new CreateEventNewsLocalizationDto
                {
                    LanguageId = 1,
                    AdditionalDescription = "Online",
                },
            ]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("CreateEventNewsDto.Localizations[0].Title");
    }
}
