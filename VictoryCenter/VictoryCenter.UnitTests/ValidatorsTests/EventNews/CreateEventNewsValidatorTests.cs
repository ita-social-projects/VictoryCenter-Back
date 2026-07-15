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
            Status = Status.Draft
        });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
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
        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.CategoryIds);
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
            CategoryIds = [1],
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
    public void Validate_ShouldHaveError_WhenCategoryIdsAreDuplicated()
    {
        var command = new CreateEventNewsCommand(new CreateEventNewsDto
        {
            Status = Status.Draft,
            CategoryIds = [1, 1]
        });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.CreateEventNewsDto.CategoryIds)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(CreateEventNewsDto.CategoryIds)));
    }
}
