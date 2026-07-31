using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;
using VictoryCenter.DAL.Entities.Localization;
using PartnersPageBannerEntity = VictoryCenter.DAL.Entities.PartnersPageBanner;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PartnersPageBanner;

public class CreatePartnersPageBannerLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization>> _mockLocalizationService;
    private readonly IValidator<CreatePartnersPageBannerLocalizationCommand> _validator;
    private readonly CreatePartnersPageBannerLocalizationHandler _handler;

    private readonly CreatePartnersPageBannerLocalizationDto _createDto = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Localized banner title",
        Description = "Localized banner description"
    };

    private readonly PartnersPageBannerLocalization _localizationEntity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Localized banner title",
        Description = "Localized banner description"
    };

    private readonly PartnersPageBannerLocalizationDto _localizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 2, Code = "en" },
        Title = "Localized banner title",
        Description = "Localized banner description"
    };

    public CreatePartnersPageBannerLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization>>();
        _validator = new CreatePartnersPageBannerLocalizationValidator(new BasePartnersPageBannerLocalizationValidator());
        _handler = new CreatePartnersPageBannerLocalizationHandler(_mockMapper.Object, _validator, _mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreatePartnersPageBannerLocalization_Successfully()
    {
        SetupDependencies();

        var result = await _handler.Handle(new CreatePartnersPageBannerLocalizationCommand(_createDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_localizationDto.Title, result.Value.Title);
        Assert.Equal(_localizationDto.EntityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenDataIsInvalid()
    {
        var invalidDto = new CreatePartnersPageBannerLocalizationDto
        {
            EntityId = 0,
            LanguageId = 0,
            Title = "",
            Description = ""
        };

        var result = await _handler.Handle(new CreatePartnersPageBannerLocalizationCommand(invalidDto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("EntityId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("LanguageId must be positive", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_createDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_localizationEntity))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var result = await _handler.Handle(new CreatePartnersPageBannerLocalizationCommand(_createDto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_createDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_localizationEntity))
            .ThrowsAsync(new InvalidOperationException());

        var result = await _handler.Handle(new CreatePartnersPageBannerLocalizationCommand(_createDto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntity(typeof(PartnersPageBannerLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_createDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_localizationEntity))
            .ThrowsAsync(new DbUpdateException());

        var result = await _handler.Handle(new CreatePartnersPageBannerLocalizationCommand(_createDto), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(PartnersPageBannerLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_createDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.CreateEntityLocalizationAsync(_localizationEntity)).ReturnsAsync(_localizationEntity);
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalizationDto>(_localizationEntity)).Returns(_localizationDto);
    }
}
