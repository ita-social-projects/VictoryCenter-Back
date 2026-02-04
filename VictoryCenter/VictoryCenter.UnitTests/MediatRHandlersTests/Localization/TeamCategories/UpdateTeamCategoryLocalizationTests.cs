using AutoMapper;
using FluentValidation;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamCategories;
public class UpdateTeamCategoryLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<TeamCategory, TeamCategoryLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdateTeamCategoryLocalizationCommand> _validator;
    private readonly UpdateTeamCategoryLocalizationHandler _handler;

    private readonly UpdateTeamCategoryLocalizationDto _updateTeamCategoryLocalizationDto = new()
    {
        Name = "Upd Localized Name",
        Description = "Upd Localized Description"
    };

    private readonly TeamCategoryLocalization _oldTeamCategoryLocalization = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Old Name",
        Description = "Old Description",
    };
    private readonly TeamCategoryLocalization _localizedTeamCategoryLocalization = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Upd Localized Name",
        Description = "Upd Localized Description",
    };
    private readonly TeamCategoryLocalizationDto _teamCategoryLocalizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 2, Code = "en" },
        Name = "Upd Localized Name",
        Description = "Upd Localized Description"
    };
    private readonly long _entityId = 1;
    private readonly long _languageId = 2;
    public UpdateTeamCategoryLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<TeamCategory, TeamCategoryLocalization>>();
        _validator = new UpdateTeamCategoryLocalizationValidator(new BaseTeamCategoryLocalizationValidator());
        _handler = new UpdateTeamCategoryLocalizationHandler(_mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTeamCategoryLocalization_Successfully()
    {
        // Arrange
        SetupDependencies();
        long entityId = _oldTeamCategoryLocalization.EntityId;
        long languageId = _oldTeamCategoryLocalization.LanguageId;
        var command = new UpdateTeamCategoryLocalizationCommand(_updateTeamCategoryLocalizationDto, entityId, languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_teamCategoryLocalizationDto.Name, result.Value.Name);
        Assert.Equal(_teamCategoryLocalizationDto.Description, result.Value.Description);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFailed()
    {
        var invalidDto = new UpdateTeamCategoryLocalizationDto
        {
            Name = "",
            Description = ""
        };

        var command = new UpdateTeamCategoryLocalizationCommand(invalidDto, _entityId, _entityId);

        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("Name is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_KeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_updateTeamCategoryLocalizationDto))
            .Returns(_localizedTeamCategoryLocalization);

        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizedTeamCategoryLocalization))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var command = new UpdateTeamCategoryLocalizationCommand(_updateTeamCategoryLocalizationDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_updateTeamCategoryLocalizationDto))
            .Returns(_localizedTeamCategoryLocalization);

        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizedTeamCategoryLocalization))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateTeamCategoryLocalizationCommand(_updateTeamCategoryLocalizationDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(TeamCategoryLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_updateTeamCategoryLocalizationDto))
            .Returns(_localizedTeamCategoryLocalization);

        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizedTeamCategoryLocalization))
            .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException());

        var command = new UpdateTeamCategoryLocalizationCommand(_updateTeamCategoryLocalizationDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(TeamCategoryLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_updateTeamCategoryLocalizationDto))
            .Returns(_localizedTeamCategoryLocalization);

        _mockMapper.Setup(m => m.Map<TeamCategoryLocalizationDto>(_localizedTeamCategoryLocalization))
            .Returns(_teamCategoryLocalizationDto);

        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizedTeamCategoryLocalization))
            .ReturnsAsync(_localizedTeamCategoryLocalization);
    }
}
