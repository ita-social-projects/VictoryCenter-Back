using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.PartnersPageBanner.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Validators.Localization.PartnersPageBanner;
using VictoryCenter.DAL.Entities.Localization;
using PartnersPageBannerEntity = VictoryCenter.DAL.Entities.PartnersPageBanner;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.PartnersPageBanner;

public class UpdatePartnersPageBannerLocalizationHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization>> _mockLocalizationService;
    private readonly IValidator<UpdatePartnersPageBannerLocalizationCommand> _validator;
    private readonly UpdatePartnersPageBannerLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly UpdatePartnersPageBannerLocalizationDto _updateDto = new()
    {
        Title = "Upd Localized banner title",
        Description = "Upd banner description"
    };

    private readonly PartnersPageBannerLocalization _localizationEntity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Upd Localized banner title",
        Description = "Upd banner description"
    };

    private readonly PartnersPageBannerLocalizationDto _localizationDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new() { Id = 2, Code = "en" },
        Title = "Upd Localized banner title",
        Description = "Upd banner description"
    };

    public UpdatePartnersPageBannerLocalizationHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLocalizationService = new Mock<ILocalizationService<PartnersPageBannerEntity, PartnersPageBannerLocalization>>();
        _validator = new UpdatePartnersPageBannerLocalizationValidator(new BasePartnersPageBannerLocalizationValidator());
        _handler = new UpdatePartnersPageBannerLocalizationHandler(_mockMapper.Object, _validator, _mockLocalizationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePartnersPageBannerLocalization_Successfully()
    {
        SetupDependencies();

        var command = new UpdatePartnersPageBannerLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_localizationDto.Title, result.Value.Title);
        Assert.Equal(_entityId, result.Value.EntityId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFailed()
    {
        var invalidDto = new UpdatePartnersPageBannerLocalizationDto
        {
            Title = "",
            Description = ""
        };

        var command = new UpdatePartnersPageBannerLocalizationCommand(invalidDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_updateDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizationEntity))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var command = new UpdatePartnersPageBannerLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_updateDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizationEntity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdatePartnersPageBannerLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(PartnersPageBannerLocalization)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_updateDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizationEntity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdatePartnersPageBannerLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(PartnersPageBannerLocalization)), result.Errors[0].Message);
    }

    private void SetupDependencies()
    {
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalization>(_updateDto)).Returns(_localizationEntity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_localizationEntity)).ReturnsAsync(_localizationEntity);
        _mockMapper.Setup(m => m.Map<PartnersPageBannerLocalizationDto>(_localizationEntity)).Returns(_localizationDto);
    }
}
