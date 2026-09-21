using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.EventNews.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Validators.EventNews;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNews;

public class UpdateEventNewsValidatorTests
{
    private readonly UpdateEventNewsValidator _validator = new(new BaseEventNewsValidator());

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenIdIsNotPositive(long id)
    {
        var command = new UpdateEventNewsCommand(id, new UpdateEventNewsDto { Status = Status.Draft });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Id);
    }

    [Fact]
    public void Validate_ShouldHaveErrors_WhenPublishedFieldsAreMissing()
    {
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto { Status = Status.Published });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.EventNews.PublishedAt);
        result.ShouldHaveValidationErrorFor(item => item.EventNews.PreviewImageId);
        result.ShouldHaveValidationErrorFor(item => item.EventNews.CategoryIds);
        result.ShouldHaveValidationErrorFor(item => item.EventNews.Localizations);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEventNewsIsNull()
    {
        var command = new UpdateEventNewsCommand(1, null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.EventNews);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDraftIsEmpty()
    {
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto { Status = Status.Draft });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedTitleIsMissing()
    {
        // Arrange
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto
        {
            Title = null,
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.EventNews.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventNewsDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedDescriptionIsMissing()
    {
        // Arrange
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto
        {
            Description = null,
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.EventNews.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateEventNewsDto.Description)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto
        {
            Title = new string('A', EventNewsConstants.TitleMaxLength + 1),
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.EventNews.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateEventNewsDto.Title), EventNewsConstants.TitleMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto
        {
            Description = new string('A', EventNewsConstants.DescriptionMaxLength + 1),
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.EventNews.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateEventNewsDto.Description), EventNewsConstants.DescriptionMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedTitleMinimumLengthIsNotMet()
    {
        // Arrange
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto
        {
            Title = new string('A', EventNewsConstants.TitleMinLength - 1),
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.EventNews.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateEventNewsDto.Title), EventNewsConstants.TitleMinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPublishedDescriptionMinimumLengthIsNotMet()
    {
        // Arrange
        var command = new UpdateEventNewsCommand(1, new UpdateEventNewsDto
        {
            Description = new string('A', EventNewsConstants.DescriptionMinLength - 1),
            Status = Status.Published,
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.EventNews.Description)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateEventNewsDto.Description), EventNewsConstants.DescriptionMinLength));
    }
}
