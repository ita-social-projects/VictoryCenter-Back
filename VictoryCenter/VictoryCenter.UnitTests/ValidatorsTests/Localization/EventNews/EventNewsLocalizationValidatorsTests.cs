using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Validators.Localization.EventNews;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.EventNews;

public class EventNewsLocalizationValidatorsTests
{
    private readonly CreateEventNewsLocalizationValidator _createValidator = new(new BaseEventNewsLocalizationValidator());
    private readonly UpdateEventNewsLocalizationValidator _updateValidator = new(new BaseEventNewsLocalizationValidator());
    private readonly DeleteEventNewsLocalizationValidator _deleteValidator = new();

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void DeleteCommand_ShouldFail_WhenIdsAreNotPositive(long entityId, long languageId)
    {
        var command = new DeleteEventNewsLocalizationCommand(entityId, languageId);

        var result = _deleteValidator.TestValidate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void DeleteCommand_ShouldPass_WhenIdsArePositive()
    {
        var command = new DeleteEventNewsLocalizationCommand(1, 1);

        var result = _deleteValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void CreateCommand_ShouldFail_WhenIdsAreNotPositive(long entityId, long languageId)
    {
        var command = new CreateEventNewsLocalizationCommand(new CreateAdminEventNewsLocalizationDto
        {
            EntityId = entityId,
            LanguageId = languageId,
            Title = "Valid Title"
        });

        var result = _createValidator.TestValidate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateCommand_ShouldFail_WhenTitleIsEmpty()
    {
        var command = new CreateEventNewsLocalizationCommand(new CreateAdminEventNewsLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            Title = string.Empty
        });

        var result = _createValidator.TestValidate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateCommand_ShouldPass_WhenValid()
    {
        var command = new CreateEventNewsLocalizationCommand(new CreateAdminEventNewsLocalizationDto
        {
            EntityId = 1,
            LanguageId = 1,
            Title = "Valid Title"
        });

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void UpdateCommand_ShouldFail_WhenIdsAreNotPositive(long entityId, long languageId)
    {
        var command = new UpdateEventNewsLocalizationCommand(
            new UpdateEventNewsLocalizationDto { Title = "Valid Title" },
            entityId,
            languageId);

        var result = _updateValidator.TestValidate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateCommand_ShouldFail_WhenTitleIsEmpty()
    {
        var command = new UpdateEventNewsLocalizationCommand(
            new UpdateEventNewsLocalizationDto { Title = string.Empty },
            1,
            1);

        var result = _updateValidator.TestValidate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateCommand_ShouldPass_WhenValid()
    {
        var command = new UpdateEventNewsLocalizationCommand(
            new UpdateEventNewsLocalizationDto { Title = "Valid Title" },
            1,
            1);

        var result = _updateValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
