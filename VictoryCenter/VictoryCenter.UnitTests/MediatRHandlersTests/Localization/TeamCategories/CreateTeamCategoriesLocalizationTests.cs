using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.TeamCategories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Localization;
namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.TeamCategories;
public class CreateTeamCategoriesLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<TeamCategory, TeamCategoryLocalization>> _mockLocalizationService;
    private readonly IValidator<CreateTeamCategoryLocalizationCommand> _validator;
    private readonly CreateTeamCategoryLocalizationHandler _handler;

    private readonly CreateTeamCategoryLocalizationDto _createTeamCategoryLocalizationDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Localized Name",
        Description = "Localized Description"
    };

    private readonly TeamCategoryLocalization _teamCategoryLocalization = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Name = "Назва категорії",
        Description = "Опис категорії",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly TeamCategoryLocalizationDto _teamCategoryLocalizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 2, Code = "en" },
        Name = "Localized Name",
        Description = "Localized Description"
    };
    public CreateTeamCategoriesLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<TeamCategory, TeamCategoryLocalization>>();
        _validator = new CreateTeamCategoryLocalizationValidator(new BaseTeamCategoryLocalizationValidator());
        _handler = new CreateTeamCategoryLocalizationHandler(_mockMapper.Object, _mockLocalizationService.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldCreateTeamCategoryLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(
            new CreateTeamCategoryLocalizationCommand(_createTeamCategoryLocalizationDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_teamCategoryLocalizationDto.Name, result.Value.Name);
        Assert.Equal(_teamCategoryLocalizationDto.LocalizationInfoDto.Id, result.Value.LocalizationInfoDto.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenDataIsInvalid()
    {
        var invalidDto = new CreateTeamCategoryLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Name = "",
            Description = ""
        };
        var result = await _handler.Handle(
            new CreateTeamCategoryLocalizationCommand(invalidDto),
            CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Name is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_createTeamCategoryLocalizationDto))
            .Returns(_teamCategoryLocalization);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_teamCategoryLocalization))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var result = await _handler.Handle(
            new CreateTeamCategoryLocalizationCommand(_createTeamCategoryLocalizationDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_createTeamCategoryLocalizationDto))
            .Returns(_teamCategoryLocalization);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_teamCategoryLocalization))
            .ThrowsAsync(new InvalidOperationException());
        var result = await _handler.Handle(
            new CreateTeamCategoryLocalizationCommand(_createTeamCategoryLocalizationDto),
            CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(TeamCategoryLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_createTeamCategoryLocalizationDto))
            .Returns(_teamCategoryLocalization);

        _mockMapper.Setup(m => m.Map<TeamCategoryLocalizationDto>(_teamCategoryLocalization))
            .Returns(_teamCategoryLocalizationDto);

        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_teamCategoryLocalization))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(
            new CreateTeamCategoryLocalizationCommand(_createTeamCategoryLocalizationDto),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(TeamCategoryLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<TeamCategoryLocalization>(_createTeamCategoryLocalizationDto))
            .Returns(_teamCategoryLocalization);

        _mockMapper.Setup(m => m.Map<TeamCategoryLocalizationDto>(_teamCategoryLocalization))
            .Returns(_teamCategoryLocalizationDto);

        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_teamCategoryLocalization))
            .ReturnsAsync(_teamCategoryLocalization);
    }
}
