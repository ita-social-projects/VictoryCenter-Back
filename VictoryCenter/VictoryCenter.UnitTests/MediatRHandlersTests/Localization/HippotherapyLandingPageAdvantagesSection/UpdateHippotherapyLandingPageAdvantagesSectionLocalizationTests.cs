using System.Transactions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyLandingPageAdvantagesSection.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Localization;
using VictoryCenter.BLL.Services.HippotherapyLandingPages;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageAdvantagesSection;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using HippotherapyLandingPageAdvantagesSectionEntity =
    VictoryCenter.DAL.Entities.HippotherapyLandingPageContents.HippotherapyLandingPageAdvantagesSection;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.Localization.HippotherapyLandingPageAdvantagesSection;

public class UpdateHippotherapyLandingPageAdvantagesSectionLocalizationTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization>> _mockLocalizationService;
    private readonly Mock<ILocalizationService<HippotherapyLandingPageAdvantageCard, HippotherapyLandingPageAdvantageCardLocalization>> _mockCardLocalizationService;
    private readonly IValidator<UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand> _validator;
    private readonly UpdateHippotherapyLandingPageAdvantagesSectionLocalizationHandler _handler;

    private readonly long _entityId = 1;
    private readonly long _languageId = 2;

    private readonly HippotherapyLandingPageAdvantagesSectionEntity _section = new()
    {
        Id = 1,
        AdvantageCards =
        [
            new HippotherapyLandingPageAdvantageCard { Id = 10, AdvantagesSectionId = 1, Description = "Original card 1", Priority = 1 },
            new HippotherapyLandingPageAdvantageCard { Id = 11, AdvantagesSectionId = 1, Description = "Original card 2", Priority = 2 },
        ],
    };

    private readonly UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto _updateDto = new()
    {
        Title = "Updated title",
        Cards =
        [
            new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 10, Description = "Updated card description 1" },
            new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 11, Description = "Updated card description 2" },
        ],
    };

    private readonly HippotherapyLandingPageAdvantagesSectionLocalization _entity = new()
    {
        EntityId = 1,
        LanguageId = 2,
        Title = "Updated title",
        Language = new LocalizationLanguage { Id = 2, Name = "English", Code = "en" },
    };

    private readonly HippotherapyLandingPageAdvantagesSectionLocalizationDto _responseDto = new()
    {
        EntityId = 1,
        LocalizationInfoDto = new LocalizationInfoDto { Id = 2, Code = "en" },
        Title = "Updated title",
    };

    public UpdateHippotherapyLandingPageAdvantagesSectionLocalizationTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAdvantagesSectionEntity, HippotherapyLandingPageAdvantagesSectionLocalization>>();
        _mockCardLocalizationService =
            new Mock<ILocalizationService<HippotherapyLandingPageAdvantageCard, HippotherapyLandingPageAdvantageCardLocalization>>();
        _validator = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
            new BaseHippotherapyLandingPageAdvantagesSectionLocalizationValidator(
                new HippotherapyLandingPageAdvantageCardLocalizationItemValidator()));

        var cardsUpdater = new HippotherapyLandingPageAdvantageCardLocalizationUpdater(
            _mockMapper.Object, _mockRepositoryWrapper.Object, _mockCardLocalizationService.Object);

        _handler = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationHandler(
            _mockMapper.Object, _mockLocalizationService.Object, _validator, _mockRepositoryWrapper.Object, cardsUpdater);

        _mockRepositoryWrapper.Setup(r => r.BeginTransaction())
            .Returns(() => new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantagesSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantagesSectionEntity>>()))
            .ReturnsAsync(_section);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocalization_Successfully()
    {
        SetupDependencies(existingCardLocalization: true);

        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_responseDto.Title, result.Value.Title);
        Assert.Equal(_entityId, result.Value.EntityId);
        Assert.Equal(new[] { 10L, 11L }, result.Value.Cards.Select(c => c.CardId));
        Assert.All(result.Value.Cards, c => Assert.Equal(TranslationStatus.Relevant, c.TranslationStatus));
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(
                It.Is<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(l => l.Count() == 2 && l.All(x => x.LanguageId == _languageId)),
                true),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateMissingCardLocalizations_WhenTheyDoNotExist()
    {
        SetupDependencies(existingCardLocalization: false);

        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(), false),
            Times.Once);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(), true),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSectionNotFound()
    {
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantagesSectionsRepository.GetFirstOrDefaultAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantagesSectionEntity>>()))
            .ReturnsAsync((HippotherapyLandingPageAdvantagesSectionEntity?)null);

        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockLocalizationService.Verify(
            s => s.UpdateEntityLocalizationAsync(It.IsAny<HippotherapyLandingPageAdvantagesSectionLocalization>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCardDoesNotBelongToSection()
    {
        SetupDependencies(existingCardLocalization: false);
        var dto = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = _updateDto.Title,
            Cards = [new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 999, Description = "Updated card description" }],
        };
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(dto)).Returns(_entity);

        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(dto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        _mockCardLocalizationService.Verify(
            s => s.TrackEntityLocalizationAsync(It.IsAny<IEnumerable<HippotherapyLandingPageAdvantageCardLocalization>>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var invalidDto = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationDto
        {
            Title = "",
            Cards = [new UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto { CardId = 0, Description = "" }],
        };
        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(invalidDto, _entityId, _languageId);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Title is required", result.Errors.Select(e => e.Message));
        Assert.Contains("CardId must be positive", result.Errors.Select(e => e.Message));
        Assert.Contains("Description is required", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenKeyNotFoundExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new KeyNotFoundException("Localization not found"));

        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Localization not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenInvalidOperationExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new InvalidOperationException());

        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntity(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionThrown()
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity))
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateHippotherapyLandingPageAdvantagesSectionLocalizationCommand(_updateDto, _entityId, _languageId);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(HippotherapyLandingPageAdvantagesSectionLocalization)),
            result.Errors[0].Message);
    }

    private void SetupDependencies(bool existingCardLocalization)
    {
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalization>(_updateDto)).Returns(_entity);
        _mockMapper.Setup(m => m.Map<HippotherapyLandingPageAdvantagesSectionLocalizationDto>(_entity)).Returns(_responseDto);
        _mockLocalizationService.Setup(s => s.UpdateEntityLocalizationAsync(_entity)).ReturnsAsync(_entity);

        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalization>(It.IsAny<UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto>()))
            .Returns((object src) =>
            {
                var dto = (UpdateHippotherapyLandingPageAdvantageCardLocalizationItemDto)src;
                return new HippotherapyLandingPageAdvantageCardLocalization
                {
                    EntityId = dto.CardId,
                    Description = dto.Description,
                    TranslationStatus = TranslationStatus.Relevant,
                };
            });
        _mockMapper
            .Setup(m => m.Map<HippotherapyLandingPageAdvantageCardLocalizationItemDto>(It.IsAny<HippotherapyLandingPageAdvantageCardLocalization>()))
            .Returns((object src) =>
            {
                var entity = (HippotherapyLandingPageAdvantageCardLocalization)src;
                return new HippotherapyLandingPageAdvantageCardLocalizationItemDto
                {
                    CardId = entity.EntityId,
                    Description = entity.Description,
                    TranslationStatus = entity.TranslationStatus,
                };
            });

        var existing = existingCardLocalization
            ? _updateDto.Cards
                .Select(c => new HippotherapyLandingPageAdvantageCardLocalization
                {
                    EntityId = c.CardId,
                    LanguageId = _languageId,
                    Description = "Old translation",
                    TranslationStatus = TranslationStatus.Outdated,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                })
                .ToList()
            : [];
        _mockRepositoryWrapper
            .Setup(r => r.HippotherapyLandingPageAdvantageCardLocalizationsRepository.GetAllAsync(
                It.IsAny<QueryOptions<HippotherapyLandingPageAdvantageCardLocalization>>()))
            .ReturnsAsync(existing);
        _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
    }
}
