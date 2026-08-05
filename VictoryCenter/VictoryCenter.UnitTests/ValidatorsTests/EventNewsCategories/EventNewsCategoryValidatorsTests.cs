using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Create;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Update;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNewsCategories;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.BLL.Validators.EventNewsCategories;
using VictoryCenter.BLL.Validators.Localization.EventNewsCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.EventNewsCategories;

public class EventNewsCategoryValidatorsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateCategory_ShouldRejectEmptyName(string? name)
    {
        var command = new CreateEventNewsCategoryCommand(
            new CreateEventNewsCategoryDto { Name = name! });

        var result = new CreateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Category.Name);
    }

    [Fact]
    public void CreateCategory_ShouldRejectNameBelowMinimumLengthAfterTrimming()
    {
        var command = new CreateEventNewsCategoryCommand(
            new CreateEventNewsCategoryDto { Name = "  a  " });

        var result = new CreateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Category.Name);
    }

    [Fact]
    public void UpdateCategory_ShouldRejectNameOverMaximumLengthAfterTrimming()
    {
        var command = new UpdateEventNewsCategoryCommand(
            1,
            new UpdateEventNewsCategoryDto
            {
                Name = $"  {new string('a', EventNewsCategoryConstants.MaxNameLength + 1)}  "
            });

        var result = new UpdateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Category.Name);
    }

    [Theory]
    [InlineData("  ab  ")]
    [InlineData("  abcdefghijklmnopqrst  ")]
    public void CreateCategory_ShouldAcceptBoundaryLengthAfterTrimming(string name)
    {
        var command = new CreateEventNewsCategoryCommand(
            new CreateEventNewsCategoryDto { Name = name });

        var result = new CreateEventNewsCategoryValidator().TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(item => item.Category.Name);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public void CreateLocalization_ShouldRejectNonPositiveIdentifiers(long entityId, long languageId)
    {
        var command = new CreateEventNewsCategoryLocalizationCommand(
            new CreateEventNewsCategoryLocalizationDto
            {
                EntityId = entityId,
                LanguageId = languageId,
                Name = "News"
            });

        var result = new CreateEventNewsCategoryLocalizationValidator().TestValidate(command);

        if (entityId <= 0)
        {
            result.ShouldHaveValidationErrorFor(item => item.Localization.EntityId);
        }

        if (languageId <= 0)
        {
            result.ShouldHaveValidationErrorFor(item => item.Localization.LanguageId);
        }
    }

    [Fact]
    public void CreateLocalization_ShouldRejectInvalidName()
    {
        var command = new CreateEventNewsCategoryLocalizationCommand(
            new CreateEventNewsCategoryLocalizationDto
            {
                EntityId = 1,
                LanguageId = 1,
                Name = new string('a', EventNewsCategoryConstants.MaxNameLength + 1)
            });

        var result = new CreateEventNewsCategoryLocalizationValidator().TestValidate(command);

        result.ShouldHaveValidationErrorFor(item => item.Localization.Name);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void UpdateLocalization_ShouldRejectNonPositiveRouteIdentifiers(long entityId, long languageId)
    {
        var command = new UpdateEventNewsCategoryLocalizationCommand(
            entityId,
            languageId,
            new UpdateEventNewsCategoryLocalizationDto { Name = "News" });

        var result = new UpdateEventNewsCategoryLocalizationValidator().TestValidate(command);

        if (entityId <= 0)
        {
            result.ShouldHaveValidationErrorFor(item => item.EntityId);
        }

        if (languageId <= 0)
        {
            result.ShouldHaveValidationErrorFor(item => item.LanguageId);
        }
    }

    [Fact]
    public void UpdateLocalization_ShouldAcceptValidRequest()
    {
        var command = new UpdateEventNewsCategoryLocalizationCommand(
            1,
            1,
            new UpdateEventNewsCategoryLocalizationDto { Name = "  News  " });

        var result = new UpdateEventNewsCategoryLocalizationValidator().TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
